using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace WebUI.Middleware
{
    public static class AuthorizationExtensions
    {
        public static IApplicationBuilder UseAuthotizationMego ( this IApplicationBuilder app )
        {
            return app.UseMiddleware<Authorization>( );
        }
    }

    public class Authorization
    {
        private readonly RequestDelegate _next;
        private readonly ApiSettings _settings;

        public Authorization ( RequestDelegate next, IOptionsMonitor<ApiSettings> settings )
        {
            _next = next;
            _settings = settings.CurrentValue;
        }

        public async Task Invoke ( HttpContext httpContext )
        {
            string authHeader = httpContext.Request.Headers["Authorization-Mego"];
            var conte = httpContext.Connection.RemoteIpAddress.ToString( );


            if (authHeader != null && authHeader.StartsWith("Auth-Mego"))
            {
                string encodeAuthorization = authHeader.Substring("Auth-Mego ".Length).Trim( );

                if (encodeAuthorization.Equals(_settings.auth_ws_identity))
                {
                    await _next(httpContext);
                }
                else
                {
                    await ResException(httpContext, "Credenciales erroneas"+ conte, Convert.ToInt32(System.Net.HttpStatusCode.Unauthorized), System.Net.HttpStatusCode.Unauthorized.ToString( ));
                }
            }
            else
            {
                await ResException(httpContext, "No autorizado", Convert.ToInt32(System.Net.HttpStatusCode.Unauthorized), System.Net.HttpStatusCode.Unauthorized.ToString( ));
            }
        }

        internal async Task ResException ( HttpContext httpContext, String infoAdicional, int statusCode, string str_res_id_servidor )
        {
            ResException respuesta = new( );

            httpContext.Response.ContentType = "application/json; charset=UTF-8";
            httpContext.Response.StatusCode = statusCode;

            respuesta.str_res_id_servidor = str_res_id_servidor;
            respuesta.str_res_codigo = "401";
            respuesta.dt_res_fecha_msj_crea = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            respuesta.str_res_estado_transaccion = "ERR";
            respuesta.str_res_info_adicional = infoAdicional;

            string str_respuesta = JsonSerializer.Serialize(respuesta);

            await httpContext.Response.WriteAsync(str_respuesta);
        }
    }
}
