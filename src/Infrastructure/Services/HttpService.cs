using Microsoft.Extensions.Options;

using System.Reflection;
using System.Text;
using System.Text.Json;

using Application.Common.Interfaces;
using Application.Common.Models;

namespace Infrastructure.Services;

public interface IHttpService
{
    Task<T> GetRestServiceDataAsync<T> ( string serviceAddress );
    Task<T> PostRestServiceDataAsync<T> ( string serializedData,
                                        string serviceAddress,
                                        string parameters,
                                        string auth,
                                        string authorizationType,
                                        string str_id_transaccion,
                                         Boolean guardarPeticion = true
                                     );

    Task<string> solicitar_servicio ( SolicitarServicio solicitarServicio );
    object solicitar_servicio_async ( SolicitarServicio solicitarServicio );
}
public class HttpService : IHttpService
{
    private readonly Dictionary<string, object> _Ilogs;
    private readonly ApiSettings _settings;
    private readonly string str_clase;
    private const string strRutaGuardarLog = "saveLogs/";
    public HttpService ( IOptionsMonitor<ApiSettings> option )
    {
        _settings = option.CurrentValue;
        str_clase = GetType( ).FullName!;

        _Ilogs = new Dictionary<string, object>( );
        _Ilogs.Add("str_base", _settings.nombre_base_mongo);
        _Ilogs.Add("str_collection", "respuestas_http");
        _Ilogs.Add("tipo_log", "respuestas_http");
        _Ilogs.Add(_settings.typeAuthAccesoLogs, _settings.auth_logs);
    }

    public async Task<T> GetRestServiceDataAsync<T> ( string serviceAddress )
    {
        try
        {
            var client = new HttpClient( );
            client.BaseAddress = new Uri(serviceAddress);
            var response = await client.GetAsync(client.BaseAddress);
            response.EnsureSuccessStatusCode( );
            var jsonResult = await response.Content.ReadAsStringAsync( );
            var result = JsonSerializer.Deserialize<T>(jsonResult)!;
            return result!;
        }
        catch
        {
            Console.WriteLine("Error");
            throw;
        }
    }


    /// <summary>
    /// Servicio de peticiónes POST
    /// </summary>
    /// <param name="serializedData"></param>
    /// <param name="parameters"></param>
    /// <param name="serviceAddress"></param>
    /// <param name="auth"></param>
    /// <param name="authorizationType"></param>
    /// <param name="str_id_transaccion"></param>
    /// <returns></returns>
    public async Task<T> PostRestServiceDataAsync<T> ( string serializedData,
                                                        string serviceAddress,
                                                        string parameters,
                                                        string auth,
                                                        string authorizationType,
                                                        string str_id_transaccion,
                                                        Boolean guardarPeticion = true )
    {
        try
        {

            HttpClient client = new( );
            client.BaseAddress = new Uri(serviceAddress);

            if (!String.IsNullOrEmpty(authorizationType))
            {
                client.DefaultRequestHeaders.Add("Authorization", authorizationType + " " + auth);
            }

            client.Timeout = TimeSpan.FromSeconds(_settings.timeOutHttp);

            var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(parameters, httpContent);
            string resultadoJson = await response.Content.ReadAsStringAsync( );

            T respuesta = default(T)!;

            if (response.IsSuccessStatusCode)
            {
                respuesta = JsonSerializer.Deserialize<T>(resultadoJson)!;
            }
            else
            {
                var res_servicio = new
                {
                    codigo = response.StatusCode,
                    cabecera = response.Headers,
                    cuerpo = response.Content.ReadAsStreamAsync( )
                };
                var data = guardarPeticion ? JsonSerializer.Deserialize<dynamic>(serializedData) : null;
                

            }

            return respuesta;
        }
        catch (Exception ex)
        {
            var data = guardarPeticion ? JsonSerializer.Deserialize<dynamic>(serializedData) : null;

            throw new Exception(data!.str_id_transaccion)!;
        }
    }
   
    public async Task<string> solicitar_servicio ( SolicitarServicio solicitarServicio )
    {
        var peticion = solicitarServicio.objSolicitud;

        try
        {
            var client = new HttpClient( );
            var request = createRequest(solicitarServicio);

            addHeaders(solicitarServicio, client);

            var response = await client.SendAsync(request);

            client.Dispose( );
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return await response.Content.ReadAsStringAsync( );
            else
                throw new ArgumentException(response.StatusCode.ToString( ));

        }
        catch (Exception ex)
        {
            saveErrorHttp(solicitarServicio, peticion, ex);
            throw new ArgumentException(ex.Message);
        }
    }

    /// <summary>
    /// Crear solicitud http
    /// </summary>
    /// <param name="solicitarServicio"></param>
    /// <returns></returns>
    private static HttpRequestMessage createRequest ( SolicitarServicio solicitarServicio )
    {
        string str_solicitud = JsonSerializer.Serialize(solicitarServicio.objSolicitud);

        var request = new HttpRequestMessage( );
        if (solicitarServicio.contentType == "application/x-www-form-urlencoded")
        {
            var parametros = JsonSerializer.Deserialize<Dictionary<string, string>>(str_solicitud)!;
            request.Content = new FormUrlEncodedContent(parametros);
        }
        else
            request.Content = new StringContent(str_solicitud, Encoding.UTF8, solicitarServicio.contentType);

        request.Method = new HttpMethod(solicitarServicio.tipoMetodo);
        request.RequestUri = new Uri(solicitarServicio.urlServicio, System.UriKind.RelativeOrAbsolute);
        request.Content.Headers.Add("No-Paging", "true");
        return request;
    }

    private static void addHeaders ( SolicitarServicio solicitarServicio, HttpClient httpClient )
    {
        if (solicitarServicio.valueAuth != null)
            httpClient.DefaultRequestHeaders.Add(solicitarServicio.tipoAuth, solicitarServicio.valueAuth);

        foreach (var header in solicitarServicio.dcyHeadersAdicionales)
        {
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToString( ));
        }
    }

    /// <summary>
    /// Guarda excepciones que den al solicitar un recurso
    /// </summary>
    /// <param name="solicitarServicio"></param>
    /// <param name="objPeticion"></param>
    /// <param name="ex"></param>
    private void saveErrorHttp ( SolicitarServicio solicitarServicio, object objPeticion, Exception ex )
    {
        solicitarServicio.objSolicitud = new { objPeticion, error = ex.Message };
        solicitarServicio.urlServicio = _settings.url_acceso_logs + strRutaGuardarLog;
        solicitarServicio.dcyHeadersAdicionales = _Ilogs;
        solicitarServicio.dcyHeadersAdicionales["str_collection"] = _settings.errores_http;
        solicitarServicio.dcyHeadersAdicionales["tipo_log"] = _settings.errores_http;
        solicitar_servicio_async(solicitarServicio);
    }

    public object solicitar_servicio_async ( SolicitarServicio solicitarServicio )
    {
        var respuesta = new object { };

        try
        {
            var client = new HttpClient( );
            var request = createRequest(solicitarServicio);

            addHeaders(solicitarServicio, client);

            client.SendAsync(request);
        }
        catch
        {
            // si se genera erro no se necesita generar excepcion
        }

        return respuesta;
    }
}