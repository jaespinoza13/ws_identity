using Microsoft.Extensions.Options;
using System.Text.Json;
using wsTokenJwt.Model;
using wsTokenJwt.Neg.Utils;

namespace wsTokenJwt.Middelware
{
    public static class ValidacionPeticionExtensions
    {
        public static IApplicationBuilder UseValidaPeticion(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestControl>();
        }
    }

    public class RequestControl
    {
        private readonly RequestDelegate _next;
        private readonly ServiceSettings _settings;
        private readonly LoadParameters _loadParameters;
        private readonly ILogger<RequestControl> _logger;

        public RequestControl(RequestDelegate next, IOptionsMonitor<ServiceSettings> settings,
                                    IOptionsMonitor<LoadParameters> optionsMonitorParam,
                                    ILogger<RequestControl> logger
                                    )

        {
            _settings = settings.CurrentValue;
            _next = next;
            _logger = logger;
            _loadParameters = optionsMonitorParam.CurrentValue;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            ValidaParametros();
            Header header = new()!;

            try
            {
                httpContext.Request.EnableBuffering();
                string str_operacion = httpContext.Request.Query["str_operacion"].ToString();


                var streaming = new StreamReader(httpContext.Request.Body);
                var raw = await streaming.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;

                await _next(httpContext);

                header = JsonSerializer.Deserialize<Header>(raw)!;
                var str_canal = LoadConfigService.FindParametro(header.str_nemonico_canal).str_nemonico;
                //bool bl_validar = _settings.lst_canales_valida_token!.Contains(str_canal);

                //Utils.control_peticion_diaria(str_operacion, _settings, header);
                //Console.WriteLine("Valido la peticion");

                //if (bl_validar)
                //{
                //    string parametros = _settings.prm_ws_acceso + "VALIDAR_TOKEN";
                //    var service = new ServiceHttp<ResComun>();
                //    ResComun respuesta = await service.PostRestServiceDataAsync(raw, _settings.servicio_ws_acceso, parametros, _settings.auth_ws_acceso, _settings);

                //    if (respuesta.str_res_codigo.Equals("000"))
                //    {
                //        await _next(httpContext);
                //        Console.WriteLine("Salio de la peticion");
                //    }
                //    else
                //    {
                //        header.str_id_transaccion = respuesta.str_id_transaccion;
                //        await ResTokenValidation(httpContext, header, respuesta.str_res_info_adicional);
                //    }
                //}
                //else
                //{
                //    await _next(httpContext);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Valido la peticion");
                await ResException(httpContext, header, ex.Message);
            }
        }

        internal async Task ResException(HttpContext httpContext, Header header, string messageException)
        {
            ResException respuesta = new();
            int statusCode = Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError);

            httpContext.Response.ContentType = "application/json; charset=UTF-8";
            httpContext.Response.StatusCode = statusCode;

            respuesta.str_res_original_id_servicio = header.str_id_servicio;
            respuesta.str_res_original_id_msj = header.str_id_msj;
            respuesta.str_res_codigo = statusCode.ToString();
            respuesta.str_res_id_servidor = messageException;
            respuesta.dt_res_fecha_msj_crea = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            respuesta.str_res_estado_transaccion = "ERR";
            respuesta.str_res_info_adicional = "Ocurrio un problema, Intente nuevamente más tarde.";

            string str_error = JsonSerializer.Serialize<ResException>(respuesta);
            _logger.LogError(statusCode, str_error);
            await httpContext.Response.WriteAsync(str_error);
        }

        internal void ValidaParametros()
        {
            if (DateTime.Compare(DateTime.Now, LoadConfigService.dt_fecha_codigos.AddDays(1)) > 0 || LoadConfigService.lst_errores.Count <= 0)
            {
                LoadConfigService.LoadConfiguration(_settings, _loadParameters);
            }
        }

        internal async Task ResTokenValidation(HttpContext httpContext, Header header, String infoAdicional)
        {
            int statusCode = Convert.ToInt32(System.Net.HttpStatusCode.Unauthorized);

            httpContext.Response.ContentType = "application/json; charset=UTF-8";
            httpContext.Response.StatusCode = statusCode;
            ResException respuesta = new();

            respuesta.str_res_original_id_servicio = header.str_id_servicio;
            respuesta.str_res_original_id_msj = header.str_id_msj;
            respuesta.str_res_codigo = statusCode.ToString();
            respuesta.str_res_id_servidor = header.str_id_transaccion;
            respuesta.dt_res_fecha_msj_crea = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            respuesta.str_res_estado_transaccion = "ERR";
            respuesta.str_res_info_adicional = "No autorizado, " + infoAdicional;

            string str_error = JsonSerializer.Serialize<ResException>(respuesta);
            _logger.LogError(statusCode, str_error);
            await httpContext.Response.WriteAsync(str_error);
        }

    }
}
