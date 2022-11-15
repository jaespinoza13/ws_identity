using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.Jwt;
using MediatR;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.LoginInvitado;

public class ResAutenticarseInvitadoInt : ResComun
{
    public string str_token { get; set; } = String.Empty;
}
public record AutenticarseInvitadoIntCommand ( Header header ) : IRequest<ResAutenticarseInvitadoInt>;

public class AutenticarseHandler : IRequestHandler<AutenticarseInvitadoIntCommand, ResAutenticarseInvitadoInt>
{
    private readonly IGenerarToken _generarToken;
    private readonly Roles _rol;
    public AutenticarseHandler ( IGenerarToken generarToken, IOptionsMonitor<Roles> options )
    {
        _generarToken = generarToken;
        _rol = options.CurrentValue;
    }

    public async Task<ResAutenticarseInvitadoInt> Handle ( AutenticarseInvitadoIntCommand request, CancellationToken cancellationToken )
    {
        var autenticarseInvitadoInterno = request.header;
        ResAutenticarseInvitadoInt respuesta = new( );
        string operaion = "GENERAR_TOKEN_INVITADO_INT";
        try
        {
            respuesta.LlenarResHeader(autenticarseInvitadoInterno);
            var claims = new ClaimsIdentity(new[]
                      {
                        new Claim( ClaimTypes.Role, _rol.InvitadoInterno),
                        new Claim( ClaimTypes.NameIdentifier,   autenticarseInvitadoInterno.str_login)
                        });
            respuesta.str_token = await _generarToken.ConstruirToken(autenticarseInvitadoInterno, operaion, claims, 5);

            respuesta.str_res_codigo = "000";
            respuesta.str_res_estado_transaccion = "OK";
            respuesta.str_res_info_adicional = "Token válido por una vez";
        }
        catch (Exception)
        {
            respuesta.str_res_codigo = "001";
            respuesta.str_res_estado_transaccion = "ERR";
            respuesta.str_res_info_adicional = "Error en la creación de token";
        }
        return respuesta;
    }
}
