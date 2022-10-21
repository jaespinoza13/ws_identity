using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.Jwt;
using MediatR;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

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
    private readonly ILogs _logsService;
    private readonly string _clase;


    private readonly IAutenticarseDat _autenticarseDat;
    private readonly ApiSettings _settings;
    private readonly IParametersInMemory _parameters;

    public AutenticarseInvitadoExtHandler ( IGenerarToken generarToken,
        IAutenticarseDat autenticarseDat, 
        IOptionsMonitor<Roles> roles,
          IOptionsMonitor<ApiSettings> options,
        ILogs logsService,
        IParametersInMemory parameters )
    {
        _clase = GetType( ).Name;

        _generarToken = generarToken;
        _rol = roles.CurrentValue;
        _autenticarseDat = autenticarseDat;
        _logsService = logsService;
        _settings = options.CurrentValue;
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
            await _logsService.SaveHeaderLogs(request.header, operaion, MethodBase.GetCurrentMethod( )!.Name, _clase);

            var claims = new ClaimsIdentity(new[]
                    {
                        new Claim( ClaimTypes.Role, _rol.InvitadoExterno),
                        new Claim( ClaimTypes.NameIdentifier,   autenticarInvitadoExterno.str_ente)
                        });
            string str_token = await _generarToken.ConstruirToken(autenticarInvitadoExterno,
                operaion,
                claims,
                Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + autenticarInvitadoExterno.str_nemonico_canal.ToUpper( )).str_valor_ini));
            if( !String.IsNullOrEmpty (str_token) && _settings.lst_canales_encriptar.Contains(autenticarInvitadoExterno.str_nemonico_canal))
            {
                var KeyCreate = CryptographyRSA.GenerarLlavePublicaPrivada( );
                var ClaveSecreta = Guid.NewGuid( ).ToString( );
                var reqAddKeys = JsonSerializer.Deserialize<ReqAddKeys>(JsonSerializer.Serialize(request.header))!;


                respuesta.str_clave_secreta = ClaveSecreta;

                reqAddKeys.str_ente = request.header.str_ente!;
                reqAddKeys.str_modulo = KeyCreate.str_modulo!;
                reqAddKeys.str_exponente = KeyCreate.str_exponente!;
                reqAddKeys.str_clave_secreta = ClaveSecreta!;
                reqAddKeys.str_llave_privada = KeyCreate.str_xml_priv!;
                reqAddKeys.str_llave_simetrica = CryptographyAES.GenerarLlaveHexadecimal(16);
                await _autenticarseDat.AddKeys(reqAddKeys);
            }
                

            

            respuesta.str_res_codigo = "000";
            respuesta.str_res_estado_transaccion = "OK";
            respuesta.str_res_info_adicional = "Token Creado Correctamente";
            await _logsService.SaveResponseLogs(respuesta, operaion, MethodBase.GetCurrentMethod( )!.Name, _clase);
            respuesta.str_token = str_token;

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
