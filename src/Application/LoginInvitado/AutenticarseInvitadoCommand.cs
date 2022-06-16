using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.Jwt;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.LoginInvitado;

public class ResAutenticarseInvitado : ResComun
{
    public string str_token { get; set; } = String.Empty;
}
public record AutenticarseInvitadoCommand ( Header header ) : IRequest<ResAutenticarseInvitado>;

public class AutenticarseHandler : IRequestHandler<AutenticarseInvitadoCommand, ResAutenticarseInvitado>
{
    private readonly IGenerarToken _generarToken;
    private readonly Roles _rol;
    public AutenticarseHandler ( IGenerarToken generarToken, IOptionsMonitor<Roles> options )
    {
        _generarToken = generarToken;
        _rol = options.CurrentValue;
    }

    public async Task<ResAutenticarseInvitado> Handle ( AutenticarseInvitadoCommand request, CancellationToken cancellationToken )
    {
        var autenticarseSoporte = request.header;
        ResAutenticarseInvitado respuesta = new( );
        string operaion = "GENERAR_TOKEN_TEMPORAL";

        try
        {
            respuesta.LlenarResHeader(autenticarseSoporte);
            respuesta.str_token = await _generarToken.ConstruirToken(autenticarseSoporte, operaion, _rol.Invitado, 1);
            respuesta.str_res_codigo = "000";
            respuesta.str_res_estado_transaccion = "OK";
            respuesta.str_res_info_adicional = "Token valido por una vez";
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
