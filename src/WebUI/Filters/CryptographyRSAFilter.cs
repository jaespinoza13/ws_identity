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


        public CryptographyRSAFilter (  IOptionsMonitor<ApiSettings> options, IMemoryCache memoryCache
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

                var reqAutenticarse = JsonSerializer.Deserialize<ReqAutenticarse>(JsonSerializer.Serialize(modelRequest.Value))!;

                    var Key = _memoryCache.Get<DatosLlaveRsa>("Key_" + reqAutenticarse.str_nemonico_canal);
                    if (Key != null)
                        try
                        {
                            reqAutenticarse.str_login = CryptographyRSA.Decrypt(reqAutenticarse.str_login, Key.str_xml_priv!);
                            reqAutenticarse.str_password = CryptographyRSA.Decrypt(reqAutenticarse.str_password, Key.str_xml_priv!);
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
