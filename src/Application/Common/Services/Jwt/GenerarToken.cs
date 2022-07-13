
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
    Task<string> ConstruirToken ( Header header, string str_operacion, ClaimsIdentity claims, Double time );
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

    public async Task<string> ConstruirToken ( Header header, string str_operacion, ClaimsIdentity claims, Double time )
    {
        ResComun respuesta = new( );
        respuesta.LlenarResHeader(header);
        string KeyCanales = _configuration["key_" + header.str_nemonico_canal.ToLower( )];
        byte[] str_key = Encoding.ASCII.GetBytes(KeyCanales);
        try
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(time),
                Issuer = _configuration["Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(str_key), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
            };
            tokenDescriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid( ).ToString("N")));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Name, header.str_nemonico_canal));
            var jwtTokenHandler = new JwtSecurityTokenHandler( );
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        catch (Exception exception)
        {
            await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase, exception);
            throw new ArgumentException(header.str_id_transaccion);
        }
    }
}