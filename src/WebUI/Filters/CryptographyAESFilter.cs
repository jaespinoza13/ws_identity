using Application.Common.Converting;
using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace WebUI.Filters
{
    public class CryptographyAESFilter : IActionFilter
    {

        private readonly IKeysDat _keysDat;
        private readonly ApiSettings _settings;

        public CryptographyAESFilter(IKeysDat keysDat, IOptionsMonitor<ApiSettings> options) { 
            _keysDat = keysDat;
            _settings = options.CurrentValue;

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

            var response = ((ObjectResult)context.Result!).Value;


            if (response != null) {
                var reqGetKeys = JsonSerializer.Deserialize<ReqGetKeys>( JsonSerializer.Serialize( response ) )!;
                if (_settings.lst_canales_encriptar.Contains( reqGetKeys.str_nemonico_canal ))
                {
                    var res_tran = _keysDat.GetKeys( reqGetKeys );
                    var Key = Conversions.ConvertConjuntoDatosToClass<ResGetKeys>( (ConjuntoDatos)res_tran.cuerpo, 0 );

                    if (Key != null)
                        try
                        {
                            _settings.lst_datos_encriptados_aes_out!.ForEach( atributo =>
                            {
                                if (response!.GetType().GetProperty( atributo ) != null)
                                {

                                    var propiedad = response.GetType().GetProperty( atributo );
                                    var valor = propiedad!.GetValue( response, null )!.ToString();
                                    var textoDesencriptado = CryptographyAES.Encrypt( valor!, Key.str_llave_simetrica );
                                    propiedad.SetValue( response, textoDesencriptado );

                                }

                            } );

                        }
                        catch (Exception)
                        {
                            throw new ArgumentException( "Error: Credenciales inválidas 002" );
                        }
                    else
                        throw new ArgumentException( "Error: Credenciales inválidas 001" );
                }
           

            }
        }


    }
}
