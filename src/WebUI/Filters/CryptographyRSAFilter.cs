using Application.Common.Converting;
using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.LogIn;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
                        var str_login = modelRequest.Value!.GetType( ).GetProperty("str_login");
                        var str_password = modelRequest.Value.GetType( ).GetProperty("str_password");
                        var valueLogin = str_login!.GetValue(modelRequest.Value, null)!.ToString( );
                        var valuePass = str_password!.GetValue(modelRequest.Value, null)!.ToString( );
                        var textoDesencriptado = CryptographyRSA.Decrypt(valueLogin!, Key.str_xml_priv!);
                        str_login.SetValue(modelRequest.Value, CryptographyRSA.Decrypt(valueLogin!, Key.str_xml_priv!));
                        str_password.SetValue(modelRequest.Value, CryptographyRSA.Decrypt(valuePass!, Key.str_xml_priv!));

                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("Error: Credenciales inválidas");
                    }
                else
                    throw new ArgumentException("Error: Credenciales inválidas");

            }




        }
        public void OnActionExecuted ( ActionExecutedContext context )
        {

        }


    }
}
