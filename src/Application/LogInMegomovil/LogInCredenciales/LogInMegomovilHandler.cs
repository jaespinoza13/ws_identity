using MediatR;

using Application.Common.Behaviours;
using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Application.Common.Cryptography;
using System.Text.Json;
using Domain.Entities;
using Application.Common.Functions;

namespace Application.LogInMegomovil.LogInCredenciales;

public class LogInMegomovilHandler : IRequestHandler<ReqValidarLogin, ResValidarLogin>
{
    private readonly ILogs _logsService;
    private readonly string str_clase;
    private readonly IAutenticarseMegomovilDat _autenticarseDat;
    private readonly IGenerarToken _generarToken;
    private readonly IParametersInMemory _parameters;
    private readonly Roles _roles;
    private readonly ApiSettings _settings;



    public LogInMegomovilHandler ( ILogs logsService,
                            IAutenticarseMegomovilDat autenticarseDat,
                            IGenerarToken generarToken,
                            IParametersInMemory parameters,
                            IOptionsMonitor<Roles> roles,
                            IOptionsMonitor<ApiSettings> options
                         )
    {
        _logsService = logsService;
        str_clase = GetType( ).FullName!;
        _autenticarseDat = autenticarseDat;
        _generarToken = generarToken;
        _parameters = parameters;
        _settings = options.CurrentValue;
        _roles = roles.CurrentValue;
    }

    public async Task<ResValidarLogin> Handle ( ReqValidarLogin reqAutenticarse, CancellationToken cancellationToken )
    {
        string str_operacion = "AUTENTICARSE";
        var respuesta = new ResValidarLogin( );
        respuesta.LlenarResHeader(reqAutenticarse);
        string password = reqAutenticarse.str_password;
        reqAutenticarse.str_password = String.Empty;
        //await _logsService.SaveHeaderLogs(reqAutenticarse, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
        reqAutenticarse.str_password = password;

        try
        {
            RespuestaTransaccion res_tran = await _autenticarseDat.getAutenticarCredenciales(reqAutenticarse);
            respuesta.str_res_codigo = res_tran.codigo;
            string str_clave = "";
            if (res_tran.codigo == "000" || res_tran.codigo == "155")
            {
                var autenticar = Conversions.ConvertConjuntoDatosToClass<DatosAutenticarMegomovil>((ConjuntoDatos)res_tran.cuerpo, 0);

                string str_ente = autenticar.lgc_ente;

                if (autenticar.int_usr_migrado == 1) autenticar.lgc_ente = "";

                string str_pass_val = autenticar.lgc_clave ?? autenticar.lgc_clave_tmp;
                string srt_pass_act = reqAutenticarse.str_password + autenticar.lgc_ente;

                bool bln_clave_valida = Functions.ValidarClave(srt_pass_act, str_pass_val);

                if (bln_clave_valida)
                {
                    var claims = new ClaimsIdentity(new[]
                        {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier, str_ente!)
                        });

                    string token = await _generarToken.ConstruirToken(reqAutenticarse,
                                                            str_operacion,
                                                            claims,
                                                            Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqAutenticarse.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                            );
                    respuesta.str_id_usuario = autenticar.lgc_pk_id.ToString( )!;
                    respuesta.str_ente = str_ente;
                    respuesta.str_token = token;
                    str_clave = BCrypt.Net.BCrypt.HashPassword(srt_pass_act);
                    res_tran.codigo = "000";
                }
                else
                {
                    res_tran.codigo = "103";
                    res_tran.diccionario["str_error"] = "Usuario o contraseña incorrectos.";
                }
            }

            respuesta.str_res_estado_transaccion = res_tran.codigo.Equals("000") ? "OK" : "ERR";
            respuesta.str_res_info_adicional = res_tran.diccionario["str_error"].ToString( );
            respuesta.str_res_codigo = res_tran.codigo;

            //await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
            respuesta.str_password = str_clave;
            return respuesta;

        }
        catch (Exception exception)
        {
            await _logsService.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
            throw new ArgumentException(reqAutenticarse.str_id_transaccion)!;
        }
    }


}


