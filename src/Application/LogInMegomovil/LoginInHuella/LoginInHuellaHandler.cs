using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Converting;
using Application.Common.Functions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.LogInMegomovil.LoginInHuella
{
    public class LoginInHuellaHandler : IRequestHandler<ReqValidarHuella, ResValidarHuella>
    {
        private readonly ILogs _logsService;
        private readonly string str_clase;
        private readonly IAutenticarseMegomovilDat _autenticarseDat;
        private readonly IGenerarToken _generarToken;
        private readonly IParametersInMemory _parameters;
        private readonly Roles _roles;
        private readonly ApiSettings _settings;



        public LoginInHuellaHandler ( ILogs logsService,
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

        public async Task<ResValidarHuella> Handle ( ReqValidarHuella reqAutenticarse, CancellationToken cancellationToken )
        {
            string str_operacion = "AUTENTICARSE";
            var respuesta = new ResValidarHuella( );
            respuesta.LlenarResHeader(reqAutenticarse);
            string password = reqAutenticarse.str_password;
            reqAutenticarse.str_password = String.Empty;
            await _logsService.SaveHeaderLogs(reqAutenticarse, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
            reqAutenticarse.str_password = password;

            try
            {
                RespuestaTransaccion res_tran = await _autenticarseDat.getAutenticarHuellaFaceID(reqAutenticarse);
                respuesta.str_res_codigo = res_tran.codigo;
                string str_clave = "";
                if (res_tran.codigo == "000")
                {
                    var autenticar = Conversions.ConvertConjuntoDatosToClass<DatosHuellaMegomovil>((ConjuntoDatos)res_tran.cuerpo, 0);

                    bool bln_clave_valida = Functions.VerificarHuella(reqAutenticarse, autenticar.lgd_huella_codigo);

                    if (bln_clave_valida)
                    {
                        var claims = new ClaimsIdentity(new[]
                            {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier, autenticar.lgc_ente!)
                        });

                        string token = await _generarToken.ConstruirToken(reqAutenticarse,
                                                                str_operacion,
                                                                claims,
                                                                Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqAutenticarse.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                                );
                        respuesta.str_id_usuario = autenticar.lgc_pk_id.ToString( )!;
                        respuesta.str_ente = autenticar.lgc_ente;
                        respuesta.str_token = token;
                        res_tran.codigo = "000";
                        str_clave = BCrypt.Net.BCrypt.HashPassword(reqAutenticarse.str_password);
                    }
                    else
                    {
                        res_tran.codigo = "170";
                        res_tran.diccionario["str_error"] = "Huella incorrecta.";
                    }
                }

                respuesta.str_res_estado_transaccion = res_tran.codigo.Equals("000") ? "OK" : "ERR";
                respuesta.str_res_info_adicional = res_tran.diccionario["str_error"].ToString( );
                respuesta.str_res_codigo = res_tran.codigo;

                await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
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
}
