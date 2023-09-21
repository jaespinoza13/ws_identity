using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Application.Common.Converting;
using Application.Common.Functions;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.Jwt;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.LogInMegomovil.Megomovil
{
    public record LoginInHuellaCommand ( object objTrama, string str_identificador, string str_clave_secreta, string str_id_transaccion ) : IRequest<object>;
    public class LoginInHuellaHandler : IRequestHandler<LoginInHuellaCommand, object>
    {
        private readonly ILogs _logsService;
        private readonly string str_clase;
        private readonly IAutenticarseMegomovilDat _autenticarseDat;
        private readonly IGenerarToken _generarToken;
        private readonly ICifradoMegomovil _cifradoMegomovil;
        private readonly IParametersInMemory _parameters;
        private readonly Roles _roles;
        bool encriptado = true;


        public LoginInHuellaHandler ( ILogs logsService,
                                IAutenticarseMegomovilDat autenticarseDat,
                                IGenerarToken generarToken,
                                IParametersInMemory parameters,
                                IOptionsMonitor<Roles> roles,
                                ICifradoMegomovil cifradoMegomovil,
                                IOptionsMonitor<ApiSettings> options
                             )
        {
            _logsService = logsService;
            str_clase = GetType( ).FullName!;
            _autenticarseDat = autenticarseDat;
            _generarToken = generarToken;
            _parameters = parameters;
            _roles = roles.CurrentValue;
            _cifradoMegomovil = cifradoMegomovil;
        }

        public async Task<object> Handle ( LoginInHuellaCommand loginInHuella, CancellationToken cancellationToken )
        {
            string str_operacion = "AUTENTICARSE";
            var respuesta = new ResValidarLogin( );
            var reqAutenticarse = new ReqValidarLogin( );

            try
            {
                reqAutenticarse  = getTramaDesencriptada(loginInHuella);

                respuesta.LlenarResHeader(reqAutenticarse);

                respuesta.str_id_transaccion = loginInHuella.str_id_transaccion;
                reqAutenticarse.str_id_transaccion = loginInHuella.str_id_transaccion;

                string password = reqAutenticarse.str_password;
                string str_huella = reqAutenticarse.str_firma_digital;
                reqAutenticarse.str_password = String.Empty;
                reqAutenticarse.str_firma_digital = "**********";
                _ = _logsService.SaveHeaderLogs(reqAutenticarse, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
                reqAutenticarse.str_password = password;
                reqAutenticarse.str_firma_digital = str_huella;

                RespuestaTransaccion res_tran = await _autenticarseDat.getAutenticarHuellaFaceID(reqAutenticarse);
                respuesta.str_res_codigo = res_tran.codigo;
                string str_clave = "";
                string token = "";
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

                        token = await _generarToken.ConstruirToken(reqAutenticarse,
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
                respuesta.dt_fecha_operacion = DateTime.Now;

                _ = _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);

                respuesta.str_token_dispositivo = reqAutenticarse.str_token_dispositivo;
                respuesta.str_token = token;
                respuesta.str_password = str_clave;
                respuesta.str_identificador = reqAutenticarse.str_identificador;
                respuesta.str_remitente = reqAutenticarse.str_remitente;
                respuesta.str_receptor = reqAutenticarse.str_receptor;
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
                if (string.IsNullOrEmpty(respuesta.str_nemonico_canal))
                {
                    respuesta.str_res_info_adicional = "identificador: " + loginInHuella.str_identificador + ", id_transaccion: " + loginInHuella.str_id_transaccion;
                }
                _ = _logsService.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
                throw new ArgumentException(reqAutenticarse.str_id_transaccion)!;
            }
        }
        private ReqValidarLogin getTramaDesencriptada ( LoginInHuellaCommand logInMegomovil )
        {
            var header = new Header( );
            header.str_id_transaccion = logInMegomovil.str_id_transaccion;
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
}
