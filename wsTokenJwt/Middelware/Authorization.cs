using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using wsTokenJwt.Model;

namespace wsTokenJwt.Middelware
{
    public class Authorization
    {
        private readonly RequestDelegate _next;
        private readonly ServiceSettings _settings;

        public Authorization(RequestDelegate next, IOptionsMonitor<ServiceSettings> settings)
        {
            _next = next;
            _settings = settings.CurrentValue;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Header header = new()!;
            try
            {
                httpContext.Request.EnableBuffering();
                var streaming = new StreamReader(httpContext.Request.Body);
                var raw = await streaming.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;

                header = JsonSerializer.Deserialize<Header>(raw)!;

                string authHeader = httpContext.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodeAuthorization = authHeader.Substring("Basic ".Length).Trim();
                    //DECODIFICA BASE 64
                    Encoding.UTF8.GetString(Convert.FromBase64String(encodeAuthorization));

                    if (encodeAuthorization.Equals(_settings.auth_ws_token_jwt))
                    {
                        await _next(httpContext);
                    }
                    else
                    {
                        await ResException(httpContext, header, "Credenciales erroneas", Convert.ToInt32(System.Net.HttpStatusCode.Unauthorized));
                    }
                }
                else
                {
                    await ResException(httpContext, header, "No autorizado", Convert.ToInt32(System.Net.HttpStatusCode.Unauthorized));
                }
            }
            catch (Exception ex)
            {
                await ResException(httpContext, header, "Ocurrio un problema, intente nuevamente más tarde (" + ex.Message + ")", Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError));
            }
        }

        internal async Task ResException(HttpContext httpContext, Header header, String infoAdicional, int statusCode)
        {
            ResException respuesta = new();

            httpContext.Response.ContentType = "application/json; charset=UTF-8";
            httpContext.Response.StatusCode = statusCode;

            respuesta.str_res_original_id_servicio = header.str_id_servicio;
            respuesta.str_res_original_id_msj = header.str_id_msj;
            respuesta.str_res_codigo = statusCode.ToString();
            respuesta.dt_res_fecha_msj_crea = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            respuesta.str_res_estado_transaccion = "ERR";
            respuesta.str_res_info_adicional = infoAdicional;

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize<ResException>(respuesta));
        }
    }
}
