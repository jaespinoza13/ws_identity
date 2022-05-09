using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using wsConsultas.Neg;
using wsTokenJwt.Model;
using wsTokenJwt.Neg.Utils;

namespace wsTokenJwt.Neg
{
    public class JsonWebTokenNeg
    {
        private readonly string str_clase;
        private readonly byte[] str_key;
        private readonly ServiceSettings _settings;
        public JsonWebTokenNeg(ServiceSettings ServiceSettings, SettingsJwt settingsJwt)
        {
            _settings = ServiceSettings;
            this.str_clase = GetType().FullName!;
            ServiceLogs.Init(ServiceSettings);
            this.str_key = Encoding.ASCII.GetBytes(settingsJwt.SecretKey);
        }

        public static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            var valid = false;
            if ((expires.HasValue && DateTime.UtcNow < expires) && (notBefore.HasValue && DateTime.UtcNow > notBefore))
            { valid = true; }


            return valid;
        }

        public ResComun ValidarToken(Header req_validar_token, string str_operacion)
        {

            var respuesta = new ResComun();
            string str_token = req_validar_token.str_token;
            req_validar_token.str_token = string.Empty;
            respuesta.LlenarResHeader(req_validar_token);
            ServiceLogs.SaveHeaderLogs(req_validar_token, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
            respuesta.str_id_transaccion = req_validar_token.str_id_transaccion;

            try
            {
                if (!String.IsNullOrEmpty(str_token))
                {
                    SecurityToken securityToken;
                    var jwtTokenHandler = new JwtSecurityTokenHandler();

                    try
                    {
                        var validationParameters = new TokenValidationParameters
                        {

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(str_key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            RequireExpirationTime = true,
                            LifetimeValidator = LifetimeValidator

                        };

                        Thread.CurrentPrincipal = jwtTokenHandler.ValidateToken(str_token, validationParameters, out securityToken);

                        respuesta.str_res_estado_transaccion = "OK";
                        respuesta.str_res_codigo = "000";
                        respuesta.str_res_info_adicional = "TÓKEN VÁLIDO";
                    }
                    catch (Exception)
                    {
                        respuesta.str_res_estado_transaccion = "OK";
                        respuesta.str_res_codigo = LoadConfigService.FindErrorCode("1005").str_valor_ini;
                        respuesta.str_res_info_adicional = LoadConfigService.FindErrorCode("1005").str_valor_fin;
                    }

                }
                else
                {
                    respuesta.str_res_estado_transaccion = "ERR";
                    respuesta.str_res_codigo = LoadConfigService.FindErrorCode("1007").str_valor_ini;
                    respuesta.str_res_info_adicional = LoadConfigService.FindErrorCode("1007").str_valor_fin;
                }


                ServiceLogs.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
                respuesta.str_token = str_token;

            }
            catch (Exception exception)
            {
                ServiceLogs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase, exception);
                throw;
            }
            return respuesta;
        }

        public ResComun GenerarToken(Header req_generar_token, string str_operacion)
        {

            var respuesta = new ResComun();
            respuesta.LlenarResHeader(req_generar_token);
            ServiceLogs.SaveHeaderLogs(req_generar_token, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
            respuesta.str_id_transaccion = req_generar_token.str_id_transaccion;

            try
            {

                Double double_time_token = Convert.ToDouble(LoadConfigService.FindParametro("TIEMPO_MAXIMO_TOKEN_" + req_generar_token.str_nemonico_canal).str_valor_ini);

                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim( "str_ip_dispositivo", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_ip_dispositivo ) ),
                        new Claim( "str_mac_dispositivo", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_mac_dispositivo )),
                        new Claim( "str_login", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_login )),
                        new Claim( "str_nemonico_canal", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_nemonico_canal )),
                        //new Claim( "str_pais", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_pais )),
                        //new Claim( "str_sesion", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_sesion )),
                        new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString( "N" ) )
                    }),

                    Expires = DateTime.UtcNow.AddMinutes(double_time_token),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(str_key), SecurityAlgorithms.HmacSha256Signature),
                    //Audience = BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_ip_dispositivo ),
                    //Issuer = BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_ip_dispositivo ),
                    IssuedAt = DateTime.UtcNow,
                };

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);

                respuesta.str_res_estado_transaccion = "OK";
                respuesta.str_res_codigo = "000";
                ServiceLogs.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
                respuesta.str_token = jwtToken;

            }
            catch (Exception exception)
            {
                respuesta.str_res_estado_transaccion = "ERR";
                respuesta.str_res_codigo = "001";
                ServiceLogs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase, exception);
                throw;
            }
            return respuesta;
        }
    }
}
