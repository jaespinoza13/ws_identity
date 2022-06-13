
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
namespace Application.Jwt;

public interface IGenerarToken
{
    Task<string> ConstruirToken ( Header header, string str_operacion, string rol, Double time );
}


internal class GenerarToken : IGenerarToken
{
    private readonly ILogs _logs;
    private readonly string _clase;
    private readonly IConfiguration _configuration;


    public GenerarToken ( IConfiguration configuration, ILogs logs )
    {
        this._logs = logs;
        this._clase = GetType( ).Name;
        _configuration = configuration;
    }

    public async Task<string> ConstruirToken ( Header header, string str_operacion, string rol, Double time )
    {
        ResComun respuesta = new( );
        respuesta.LlenarResHeader(header);

        Double double_time_token = header.str_nemonico_canal.Equals("CANVEN") ? 1 : time;
        string KeyCanales = _configuration["key_" + header.str_nemonico_canal.ToLower( )];
        byte[] str_key = Encoding.ASCII.GetBytes(KeyCanales);


        await _logs.SaveHeaderLogs(header, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase);

        try
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim( ClaimTypes.Role,  rol),
                        new Claim( ClaimTypes.NameIdentifier,  header.str_id_usuario),
                        new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString( "N" ) )
                    }),

                Expires = DateTime.UtcNow.AddMinutes(double_time_token),
                Issuer = _configuration["Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(str_key), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler( );
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            await _logs.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase);
            return jwtTokenHandler.WriteToken(token);
        }
        catch (Exception exception)
        {
            await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase, exception);
            throw new ArgumentException(header.str_id_transaccion);
        }
    }
}