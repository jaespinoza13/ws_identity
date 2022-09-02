
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
        string KeyCanales = _configuration["key_" + header.str_nemonico_canal.ToLower( )];
        byte[] str_key = Encoding.ASCII.GetBytes(KeyCanales);
        var privateKeyBytes = Convert.FromBase64String("MIIEowIBAAKCAQEAyokLGrnqRUToy6kYNe/WyY3d0WEzXaUK+G2M8h4xtKGfPIYR\nTKPZvX93D6IgoyqZxfvjeyXDJGUfZqskBTwSq+j+cy3X0xDGe8pL+FUavvuV2BkN\ndPWKWxqoy0PKW0GaaT3wUQOQNQmxKTgryHIeT+n/97lmZNx4K2p5z6bhIpj6ZltQ\n4O5FWGswtUM0wWk7Svw/Tk9Br6W4OlEcudXPA99swy7JW0BmKgKgnTbt9V7hhUx2\n2BiQE19XGlFB5mKxRz1CaePuv3b9EuO16Ym/Dg60Ex4RtNpbu6nlKDFXffgCH09T\nj8bmDO/Dk7PMJHe9f5UULZekRBz7DeJg09I2/wIDAQABAoIBACaoY7s1MzcHgRum\nad2ZqriL4IPfdqtwHhju6BEqnDgrBTbLLDAhsiTOWI5eVrZuIi28912BBq9Cseyp\ny9VH8xRnA3I+lMxPjmMAaOG8dL9xS9eUaJIjb8YV35P0m4IxkmR2ExGTiYnmEK+a\nbhjzVz/PnTDObXHg9vrqdtegtaYhj9ikw2w48owciuBsD3Bg03xMUwHbynAUG3Av\njU5LD5bfuEpGumDNnPy6AXFp2NJEeeq9is2Y0eFxgnRJNAljjzMZPoRkwCVAsy8g\nbuXyMTiCYZEx6acF4HWwiMtrGurL2HildWzQkd2VLOrureHw6T4vgios9S65AjAL\nv4gJcCkCgYEA74KbMp2SYuAq0pQ0kPClL6roT7/4MZ0DHt0nUlviCdRm2D9K5fkC\ncIAQQnWT2e6bVHbhw3vkjmG0o0eUL6VyMNvfibGhn+yzDU1gpD32bNsoRU5IeuMQ\nodhg6aOPFlPvDTNmbidMTvL9QaD7RHfr4KJIHDkGh2+ElB6KBGgwUnMCgYEA2HrA\nTRFF2SgmB3We9rtVT4bJZSAOkZmiI2VpM8loZ3nwEs5x3OKpaBQEs4HClwWxjuoK\nZoKfyZKnaNz1LjJ1VGKZDugKONfyPZWCIIprfgVkviRQwJHnsqbppTeQTLVohidH\n6jY8m4TLrL0haxJDT0NoLiIj9srEoR3X8zu+ikUCgYEArDyXEPIhqDsecGql1qlH\nkRztjRQ3Dq6j5NkTAvYSehElmFMDsJe+elqN1s0o8urVBwuq1OJOfVmkBlteJFls\n4dfsS9/So+ga5vEDE3l/sc50ikp+cujBODIbl0jIiDz5xtt0yLg39vpkx4JVz2oR\n1Wu+QZV8rX6zr7S6eerW/SMCgYBfz8R416JAgKKEPqzCqxsQ/aj5VvzbuFGotOOh\nBg1tbuywhiqjBrbP17xU7qN/UAfMJw2/XST3hC8QHGtvrOl9Fb6EeHK9weX3F8rm\nOB1nQ1/ZQB11fZ481d8nPrZhHRFL/uq3YJXmhxnWNEcsKoMb+8uKT5X3TrtES/8e\nKl0kuQKBgBqsCtM2jyLJ88cF6T2kr2Zt+tcUCz2WJEJlOAYymj/nid/rIwQLEpak\nnI9LzIm6q7z3vb/dRb8nl56S5sN4z0pO3v8PCfsDofTQaSANIbOLMvKwfIW01ts9\nZXmKQ7kTqMt2GwN0uIXPt4W7rxcScaG1WivamJevWIrH/htpSL5s");
        RSA rsa = RSA.Create(2048);
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        var key = new RsaSecurityKey(rsa);
        var securityKey1 = new SymmetricSecurityKey(Encoding.Default.GetBytes("ProEMLh5e_qnzdNU"));

        var ep = new EncryptingCredentials(
            securityKey1,
            SecurityAlgorithms.Aes128KW,
            SecurityAlgorithms.Aes128CbcHmacSha256);

        try
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(time),
                Issuer = _configuration["Issuer"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256),
                IssuedAt = DateTime.UtcNow,
                EncryptingCredentials = ep
            };
            tokenDescriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid( ).ToString("N")));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Name, header.str_nemonico_canal));
            var jwtTokenHandler = new JwtSecurityTokenHandler( );
            var token = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
           
            return jwtTokenHandler.WriteToken(token);
        }
        catch (Exception exception)
        {
            await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase, exception);
            throw new ArgumentException(header.str_id_transaccion);
        }
    }
}