
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
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
        var privateKeyBytes = Convert.FromBase64String(_configuration["key_token_pri"]);
        var rsa = RSA.Create(2048);
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        var key = new RsaSecurityKey(rsa);
        var securityKeyEncrypt = new SymmetricSecurityKey(Encoding.Default.GetBytes(_configuration["key_encrypt_token"]));


        try
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(time),
                Issuer = _configuration["Issuer"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256),
                IssuedAt = DateTime.UtcNow,
                EncryptingCredentials = new EncryptingCredentials(
                                        securityKeyEncrypt,
                                        SecurityAlgorithms.Aes128KW,
                                        SecurityAlgorithms.Aes128CbcHmacSha256)
            };
            tokenDescriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid( ).ToString("N")));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Name, header.str_nemonico_canal));
            var jwtTokenHandler = new JwtSecurityTokenHandler( );
            var token = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
        catch (Exception exception)
        {
            _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase, exception);
            throw new ArgumentException(header.str_id_transaccion);
        }
    }
}