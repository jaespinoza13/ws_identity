
using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace Application.Acceso.RecuperarContrasenia;

public class ValidaInfoHandler : IRequestHandler<ReqValidaInfo, ResValidaInfo>
{

    private readonly ILogs _logs;
    private readonly IAccesoDat _accesoDat;
    private readonly string _clase;
    private readonly IWsOtp _wsOtp;
    private readonly IParametersInMemory _parametersInMemory;
    private readonly byte[] str_key;

    public ValidaInfoHandler ( ILogs logs, IAccesoDat accesoDat, IWsOtp wsOtp, IParametersInMemory parametersInMemory )
    {
        _logs = logs;
        _accesoDat = accesoDat;
        _clase = GetType( ).Name;
        _wsOtp = wsOtp;
        _parametersInMemory = parametersInMemory;
        this.str_key = Encoding.ASCII.GetBytes(option.CurrentValue.key_canbvi);
    }

    public async Task<ResValidaInfo> Handle ( ReqValidaInfo reqValidaInfo, CancellationToken cancellationToken )
    {
        string str_operacion = "VALIDAR_INFO_RECUPERACION";
        ResValidaInfo respuesta = new( );
        respuesta.LlenarResHeader(reqValidaInfo);
        string token = String.Empty;

        try
        {

            RespuestaTransaccion resTran = await _accesoDat.ValidaInfoRecuperacion(reqValidaInfo);

            if (resTran.codigo.Equals("000"))
            {
                respuesta.datos_recuperacion = Conversions.ConvertConjuntoDatosToClass<DatosRecuperacion>((ConjuntoDatos)resTran.cuerpo, 0)!;

                reqValidaInfo.str_ente = respuesta.datos_recuperacion.int_ente + String.Empty;
                reqValidaInfo.str_id_usuario = respuesta.datos_recuperacion.int_id_usuario + String.Empty;

                respuesta.datos_recuperacion.bl_requiere_otp = _wsOtp.ValidaRequiereOtp(reqValidaInfo, reqValidaInfo.str_id_servicio).Result.codigo.Equals("1009");
                respuesta.str_res_estado_transaccion = "OK";

                

                Double double_time_token = Convert.ToDouble(_parametersInMemory.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqValidaInfo.str_nemonico_canal).str_valor_ini);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim( ClaimTypes.Role,  Rol.Usuario),
                        new Claim( ClaimTypes.NameIdentifier,  reqValidaInfo.str_id_usuario),
                        new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString( "N" ) )
                    }),

                    Expires = DateTime.UtcNow.AddMinutes(double_time_token),
                    Issuer = "CoopMego",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(str_key), SecurityAlgorithms.HmacSha256Signature),
                    IssuedAt = DateTime.UtcNow,
                };

                var jwtTokenHandler = new JwtSecurityTokenHandler( );
                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                await _logs.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);

                return jwtTokenHandler.WriteToken(token);

            }
            else
            {
                respuesta.str_res_estado_transaccion = "ERR";
            }

            respuesta.str_res_codigo = resTran.codigo;
            respuesta.str_res_info_adicional = resTran.diccionario["str_error"].ToString( );

            await _logs.SaveResponseLogs(reqValidaInfo, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase);
            respuesta.str_token = token;

            return respuesta;
        }
        catch (Exception exception)
        {
            await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase, exception);
            throw new ArgumentException(respuesta.str_id_transaccion);
        }
    }
}