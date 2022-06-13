
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.Jwt;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.LoginWallet;

public class ResAutenticarseWallet : ResComun
{
    public string str_token { get; set; } = String.Empty;
}


public record ReqAutenticarseCommand ( Header header ) : IRequest<ResAutenticarseWallet>;

public class AutenticarseWalletHander : IRequestHandler<ReqAutenticarseCommand, ResAutenticarseWallet>
{
    private readonly IGenerarToken _generarToken;
    private readonly Roles _rol;
    private readonly IParametersInMemory _parameters;
    public AutenticarseWalletHander ( IGenerarToken generarToken, IOptionsMonitor<Roles> options, IParametersInMemory parameters )
    {
        _generarToken = generarToken;
        _rol = options.CurrentValue;
        _parameters = parameters;
    }

    public async Task<ResAutenticarseWallet> Handle ( ReqAutenticarseCommand request, CancellationToken cancellationToken )
    {
        var autenticarseWallet = request.header;
        ResAutenticarseWallet respuesta = new( );
        respuesta.LlenarResHeader(autenticarseWallet);
        respuesta.str_token = await _generarToken.ConstruirToken(autenticarseWallet,
                                                                "AUTENTICARSE_WALLET",
                                                                _rol.Socio,
                                                                Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + autenticarseWallet.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                                );
        return respuesta;
    }
}
