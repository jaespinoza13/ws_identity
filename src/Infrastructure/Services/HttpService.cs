﻿using Microsoft.Extensions.Options;

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

    object solicitar_servicio_async ( SolicitarServicio solicitarServicio );
}
public class HttpService : IHttpService
{
    private readonly ApiSettings _settings;
    private readonly string str_clase;
    public HttpService ( IOptionsMonitor<ApiSettings> option )
    {
        _settings = option.CurrentValue;
        str_clase = GetType( ).FullName!;
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