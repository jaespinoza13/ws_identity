using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.RecuperarReenvio
{
    public class ValidarInfRecupReenvioHandler : IRequestHandler<ReqValidarInfRecupReenvio, ResValidarInfRecupReenvio>
    {
        private readonly string _clase;
        private readonly Roles _rol;
        private readonly ILogs _logs;
        private readonly IValidarRecuperaciones _accesoDat;
        private readonly IWsOtp _wsOtp;
        private readonly IParametersInMemory _parameters;
        private readonly IGenerarToken _generarToken;

        public ValidarInfRecupReenvioHandler ( ILogs logs, IValidarRecuperaciones validarRecuperacionesDat, IWsOtp wsOtp, IParametersInMemory parametersInMemory, IOptionsMonitor<Roles> options, IGenerarToken generarToken )
        {
            _logs = logs;
            _accesoDat = validarRecuperacionesDat;
            _clase = GetType( ).Name;
            _wsOtp = wsOtp;
            _parameters = parametersInMemory;
            _rol = options.CurrentValue;
            _generarToken = generarToken;
        }

        public async Task<ResValidarInfRecupReenvio> Handle ( ReqValidarInfRecupReenvio reqValidarInfRecupReenvio, CancellationToken cancellationToken )
        {
            string str_operacion = "VALIDAR_INF_RECUP_REENVIO";
            var respuesta = new ResValidarInfRecupReenvio( );
            respuesta.LlenarResHeader(reqValidarInfRecupReenvio);
            await _logs.SaveHeaderLogs(reqValidarInfRecupReenvio, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase);

            string token = String.Empty;

            try
            {
                RespuestaTransaccion res_tran = _accesoDat.ValidarInfRecupReactiva(reqValidarInfRecupReenvio);

                if (res_tran.codigo.Equals("000"))
                {

                    respuesta.datos_recuperacion = Conversions.ConvertConjuntoDatosToClass<DatosRecuperacion>((ConjuntoDatos)res_tran.cuerpo, 0)!;
                    reqValidarInfRecupReenvio.str_ente = respuesta.datos_recuperacion.int_ente + String.Empty;
                    reqValidarInfRecupReenvio.str_id_usuario = respuesta.datos_recuperacion.int_id_usuario + String.Empty;
                    respuesta.datos_recuperacion.bl_requiere_otp = _wsOtp.ValidaRequiereOtp(reqValidarInfRecupReenvio, reqValidarInfRecupReenvio.str_id_servicio).Result;
                    respuesta.str_res_estado_transaccion = "OK";
                    var claims = new ClaimsIdentity(new[]
                          {
                        new Claim( ClaimTypes.Role,  _rol.InvitadoExterno),
                        new Claim( ClaimTypes.NameIdentifier, reqValidarInfRecupReenvio.str_ente)
                        });
                    token = await _generarToken.ConstruirToken(reqValidarInfRecupReenvio, str_operacion, claims, Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqValidarInfRecupReenvio.str_nemonico_canal.ToUpper( )).str_valor_ini));
                }
                else
                {
                    respuesta.str_res_estado_transaccion = "ERR";
                }

                respuesta.str_res_codigo = res_tran.codigo;
                respuesta.str_res_info_adicional = res_tran.diccionario["str_error"].ToString( );

                await _logs.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase);
                respuesta.datos_recuperacion.str_token = token;

                return respuesta;
            }
            catch (Exception exception)
            {
                await _logs.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, _clase, exception);
                throw new ArgumentException(reqValidarInfRecupReenvio.str_id_transaccion)!;
            }
        }      
    }
}
