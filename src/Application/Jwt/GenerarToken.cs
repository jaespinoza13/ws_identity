
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
    Task<string> ConstruirToken ( Header header, string str_operacion );
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


    public async Task<string> ConstruirToken ( Header header, string str_operacion )
    {
        var respuesta = new ResComun( );
        respuesta.LlenarResHeader(header);
        await _logs.SaveHeaderLogs(header, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);

        try
        {
            Double double_time_token = Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + header.str_nemonico_canal).str_valor_ini);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim( ClaimTypes.Role,  Rol.Socio),
                        new Claim( ClaimTypes.NameIdentifier,  header.str_id_usuario),
                        new Claim( ClaimTypes.Hash,  header.str_login),
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
        catch (Exception exception)
        {
            await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
            throw new ArgumentException(header.str_id_transaccion);
        }
    }
}



