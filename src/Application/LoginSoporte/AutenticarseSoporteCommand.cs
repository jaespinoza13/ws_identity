using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Jwt;
using MediatR;

namespace Application.LoginSoporte;

public class ResAutenticarseSoporte : ResComun
{
    public string str_token { get; set; } = String.Empty;
}
public record AutenticarseSoporteCommand ( Header header ) : IRequest<ResAutenticarseSoporte>;

public class AutenticarseHandler : RequestHandler<AutenticarseSoporteCommand, ResAutenticarseSoporte>
{
    private readonly IGenerarToken _generarToken;
    private readonly ILogs _logs;
    public AutenticarseHandler ( IGenerarToken generarToken, ILogs logs )
    {
        _generarToken = generarToken;
        _logs = logs;
    }

    protected override ResAutenticarseSoporte Handle ( AutenticarseSoporteCommand request )
    {
        var autenticarseSoporte = request.header;
        ResAutenticarseSoporte respuesta = new( );
        respuesta.LlenarResHeader(autenticarseSoporte);

        respuesta.str_token = _generarToken.ConstruirToken(autenticarseSoporte, "AUTENTICARSE_SOPORTE").Result;
        return respuesta;
    }
}
