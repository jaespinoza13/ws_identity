using Application.Common.Converting;
using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.LogIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using static Application.Common.Cryptography.CryptographyRSA;

namespace WebUI.Filters
{
    public class CryptographyRSAFilter : IActionFilter
    {

        private readonly ApiSettings _settings;
        private readonly IMemoryCache _memoryCache;


        public CryptographyRSAFilter ( IOptionsMonitor<ApiSettings> options, IMemoryCache memoryCache
 )
        {
            _settings = options.CurrentValue;
            _memoryCache = memoryCache;

        }

        public void OnActionExecuting ( ActionExecutingContext context )
        {
            var modelRequest = context.ActionArguments.First( );
            var reqGetKeys = JsonSerializer.Deserialize<ReqGetKeys>(JsonSerializer.Serialize(modelRequest.Value))!;

            var solicitudBody = modelRequest.Value;
            if (_settings.lst_canales_encriptar.Contains(reqGetKeys.str_nemonico_canal))
            {


                var Key = _memoryCache.Get<DatosLlaveRsa>("Key_" + reqGetKeys.str_nemonico_canal);
                if (Key != null)
                    try
                    {
                        modelRequest.Value!.GetType( ).GetMethod("EncryptAES")!.Invoke(modelRequest.Value, new object[] { Key });

                    }
                    catch (Exception)
                    {
                        GenerarExcepcion(context);
                    }
                else
                    GenerarExcepcion(context);

            }




        }
        public void OnActionExecuted ( ActionExecutedContext context )
        {

        }

        public void GenerarExcepcion ( ActionExecutingContext context )
        {
            ResException resException = new( );
            resException.str_res_codigo = Convert.ToInt32(HttpStatusCode.Unauthorized).ToString( );
            resException.str_res_id_servidor = "Error: Credenciales inválidas";
            resException.str_res_estado_transaccion = "ERR";
            resException.dt_res_fecha_msj_crea = DateTime.Now;
            resException.str_res_info_adicional = "Tu sesión ha caducado, por favor ingresa nuevamente.";

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Result = new ObjectResult(resException);
        }
    }
}
