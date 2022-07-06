using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.Jwt;
using MediatR;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.LoginInvitado;

public class ResAutenticarseInvitadoExt : ResComun
{
    public string str_token { get; set; } = String.Empty;
}
public record AutenticarseInvitadoExtCommand ( Header header ) : IRequest<ResAutenticarseInvitadoExt>;

public class AutenticarseInvitadoExtHandler : IRequestHandler<AutenticarseInvitadoExtCommand, ResAutenticarseInvitadoExt>
{
    private readonly IGenerarToken _generarToken;
    private readonly Roles _rol;
    private readonly IParametersInMemory _parameters;

    public AutenticarseInvitadoExtHandler ( IGenerarToken generarToken, IOptionsMonitor<Roles> options, IParametersInMemory parameters )
    {
        _generarToken = generarToken;
        _rol = options.CurrentValue;

        _parameters = parameters;
    }

    public async Task<ResAutenticarseInvitadoExt> Handle ( AutenticarseInvitadoExtCommand request, CancellationToken cancellationToken )
    {
        var autenticarInvitadoExterno = request.header;
        ResAutenticarseInvitadoExt respuesta = new( );
        string operaion = "GENERAR_TOKEN_INVITADO_EXT";

        try
        {
            respuesta.LlenarResHeader(autenticarInvitadoExterno);
            var claims = new ClaimsIdentity(new[]
                    {
                        new Claim( ClaimTypes.Role, _rol.InvitadoExterno),
                        new Claim( ClaimTypes.NameIdentifier,   autenticarInvitadoExterno.str_ente)
                        });
            respuesta.str_token = await _generarToken.ConstruirToken(autenticarInvitadoExterno,
                operaion,
                claims,
                Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + autenticarInvitadoExterno.str_nemonico_canal.ToUpper( )).str_valor_ini));
            respuesta.str_res_codigo = "000";
            respuesta.str_res_estado_transaccion = "OK";
            respuesta.str_res_info_adicional = "Token Creado Correctamente";
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
