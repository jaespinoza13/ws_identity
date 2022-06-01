
using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
namespace Application.Jwt;

public interface IGenerarToken
{
    Task<string> ConstruirToken ( Header req_generar_token, string str_operacion );
}


internal class GenerarToken : IGenerarToken
{
    private readonly IParametersInMemory _parameters;
    private readonly ILogs _logs;
    private readonly string str_clase;
    private readonly byte[] str_key;
    public GenerarToken ( IOptionsMonitor<SecurityKeys> option, IParametersInMemory parameters, ILogs logs )
    {
        this._parameters = parameters;
        this._logs = logs;
        this.str_clase = GetType( ).Name;
        this.str_key = Encoding.ASCII.GetBytes(option.CurrentValue.key_canbvi);
    }


    public async Task<string> ConstruirToken ( Header req_generar_token, string str_operacion )
    {
        var respuesta = new ResComun( );
        respuesta.LlenarResHeader(req_generar_token);
        await _logs.SaveHeaderLogs(req_generar_token, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);

        try
        {

            Double double_time_token = Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + req_generar_token.str_nemonico_canal).str_valor_ini);

            var encryptIpDispositivo = BCrypt.Net.BCrypt.HashPassword(req_generar_token.str_ip_dispositivo);

            var jwtTokenHandler = new JwtSecurityTokenHandler( );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim( "str_ip_dispositivo",  encryptIpDispositivo),
                        new Claim( "str_mac_dispositivo", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_mac_dispositivo )),
                        new Claim( "str_login", BCrypt.Net.BCrypt.HashPassword( req_generar_token.str_login )),
                        new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString( "N" ) )
                    }),

                Expires = DateTime.UtcNow.AddMinutes(double_time_token),
                Audience = encryptIpDispositivo,
                Issuer = encryptIpDispositivo,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(str_key), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            await _logs.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);

            return jwtTokenHandler.WriteToken(token);

        }
        catch (Exception exception)
        {
            await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
            throw new ArgumentException(req_generar_token.str_id_transaccion);
        }
    }
}



