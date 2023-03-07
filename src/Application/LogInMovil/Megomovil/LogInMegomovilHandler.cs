using MediatR;
using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using Domain.Entities;
using Application.Common.Functions;
using Application.Common.ISO20022.Models;

namespace Application.LogInMegomovil.Megomovil;

public record LogInMegomovilCommand ( object objTrama, string str_identificador, string str_clave_secreta ) : IRequest<object>;
public class LogInMegomovilHandler : IRequestHandler<LogInMegomovilCommand, object>
{
    private readonly ILogs _logsService;
    private readonly string str_clase;
    private readonly IAutenticarseMegomovilDat _autenticarseDat;
    private readonly ICifradoMegomovil _cifradoMegomovil;
    private readonly IGenerarToken _generarToken;
    private readonly IParametersInMemory _parameters;
    private readonly Roles _roles;
    private readonly ApiSettings _settings;
    bool encriptado = true;
    public LogInMegomovilHandler ( ILogs logsService,
                            IAutenticarseMegomovilDat autenticarseDat,
                            IGenerarToken generarToken,
                            IParametersInMemory parameters,
                            IOptionsMonitor<Roles> roles,
                            IOptionsMonitor<ApiSettings> options,
                            ICifradoMegomovil cifradoMegomovil
                         )
    {
        _logsService = logsService;
        str_clase = GetType( ).FullName!;
        _autenticarseDat = autenticarseDat;
        _generarToken = generarToken;
        _parameters = parameters;
        _settings = options.CurrentValue;
        _roles = roles.CurrentValue;
        _cifradoMegomovil = cifradoMegomovil;
    }

    public async Task<object> Handle ( LogInMegomovilCommand logInMegomovilCommand, CancellationToken cancellationToken )
    {
        var respuesta = new ResValidarLogin( );
        var reqAutenticarse = new ReqValidarLogin( );
        string str_operacion = "AUTENTICARSE";

        try
        {
            reqAutenticarse = getTramaDesencriptada(logInMegomovilCommand);

            respuesta.LlenarResHeader(reqAutenticarse);
            string password = reqAutenticarse.str_password;
            reqAutenticarse.str_password = String.Empty;
            await _logsService.SaveHeaderLogs(reqAutenticarse, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
            reqAutenticarse.str_password = password;

            RespuestaTransaccion res_tran = await _autenticarseDat.getAutenticarCredenciales(reqAutenticarse);
            respuesta.str_res_codigo = res_tran.codigo;
            string str_clave = "";
            string token = "";
            if (res_tran.codigo == "000" || res_tran.codigo == "155")
            {
                var autenticar = Conversions.ConvertConjuntoDatosToClass<DatosAutenticarMegomovil>((ConjuntoDatos)res_tran.cuerpo, 0);

                string str_ente = autenticar.lgc_ente;
                string str_id_usuario = autenticar.lgc_pk_id.ToString( )!;

                if (autenticar.int_usr_migrado == 1) autenticar.lgc_ente = "";

                string str_pass_val = String.IsNullOrEmpty(autenticar.lgc_clave.Trim( )) ? autenticar.lgc_clave_tmp : autenticar.lgc_clave;
                string srt_pass_act = reqAutenticarse.str_password + autenticar.lgc_ente;

                bool bln_clave_valida = Functions.ValidarClave(srt_pass_act, str_pass_val);

                if (bln_clave_valida)
                {
                    var claims = new ClaimsIdentity(new[]
                        {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier, str_ente!),
                        new Claim( ClaimTypes.Sid, str_id_usuario)
                        });

                    token = await _generarToken.ConstruirToken(reqAutenticarse,
                                                            str_operacion,
                                                            claims,
                                                            Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqAutenticarse.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                            );
                    respuesta.str_id_usuario = str_id_usuario;
                    respuesta.str_ente = str_ente;
                    str_clave = BCrypt.Net.BCrypt.HashPassword(srt_pass_act);
                    res_tran.codigo = "000";
                }
                else
                {
                    res_tran.codigo = "103";
                    res_tran.diccionario["str_error"] = "Usuario o contraseña incorrectos.";
                }
            }

            respuesta.str_res_codigo = res_tran.codigo;
            respuesta.str_res_estado_transaccion = res_tran.codigo.Equals("000") ? "OK" : "ERR";
            respuesta.str_res_info_adicional = res_tran.diccionario["str_error"].ToString( );

            await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);

            respuesta.str_token_dispositivo = reqAutenticarse.str_token_dispositivo;
            respuesta.str_token = token;
            respuesta.str_password = str_clave;
            respuesta.str_identificador = reqAutenticarse.str_identificador;

            var result = new
            {
                trama = getTramaEncriptada(respuesta),
                code = respuesta.str_res_codigo,
                token = respuesta.str_token
            };

            return result;

        }
        catch (Exception exception)
        {
            await _logsService.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
            throw new ArgumentException(reqAutenticarse.str_id_transaccion)!;
        }
    }

    private ReqValidarLogin getTramaDesencriptada ( LogInMegomovilCommand logInMegomovil )
    {
        var header = new Header( );
        header.str_id_transaccion = Guid.NewGuid( ).ToString( );
        header.str_id_servicio = "REQ_VALIDAR_LOGIN";
        header.str_nemonico_canal = "CANBMO";
        
        string str_result = "";

        if (encriptado)
        {
            _cifradoMegomovil.getLlavesCifrado(header, logInMegomovil.str_identificador, logInMegomovil.str_clave_secreta);
            var dicTrama = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(logInMegomovil.objTrama))!;
            str_result = _cifradoMegomovil.desencriptarTrama(dicTrama["data"]);
        }
        else
            str_result = JsonSerializer.Serialize(logInMegomovil.objTrama);

        var reqAutenticar = JsonSerializer.Deserialize<ReqValidarLogin>(str_result)!;

        return reqAutenticar;
    }
    private string getTramaEncriptada ( ResValidarLogin respuesta )
    {
        string str_tram = JsonSerializer.Serialize(respuesta);
        return encriptado ? _cifradoMegomovil.encriptarTrama(str_tram) : str_tram;
    }

}


