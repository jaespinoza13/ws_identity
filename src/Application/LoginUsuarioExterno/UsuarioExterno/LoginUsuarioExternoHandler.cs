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
using Application.Common.Functions;


namespace Application.LoginUsuarioExterno.UsuarioExterno
{
    public class LoginUsuarioExternoHandler: IRequestHandler<ReqLoginUsuarioExterno, ResLoginUsuarioExterno>
    {
        private readonly ILogs _logsService;
        private readonly string str_clase;
        private readonly IAutenticarseUsuarioExternoDat _autenticarseUsuariosExternosDat;
        private readonly IEncryptMego _encrypt;
        private readonly IGenerarToken _generarToken;
        private readonly IParametersInMemory _parameters;
        private readonly Roles _roles;
        private readonly ApiSettings _settings;

        public LoginUsuarioExternoHandler ( ILogs logsService,
                           IAutenticarseUsuarioExternoDat autenticarseUsuariosExternosDat,
                           IEncryptMego encrypt,
                           IGenerarToken generarToken,
                           IParametersInMemory parameters,
                           IOptionsMonitor<Roles> roles,
                           IOptionsMonitor<ApiSettings> options
                        )
        {
            _logsService = logsService;
            str_clase = GetType( ).FullName!;
            _autenticarseUsuariosExternosDat = autenticarseUsuariosExternosDat;
            _encrypt = encrypt;
            _generarToken = generarToken;
            _parameters = parameters;
            _settings = options.CurrentValue;
            _roles = roles.CurrentValue;
        }

        public async Task<ResLoginUsuarioExterno> Handle ( ReqLoginUsuarioExterno reqLoginUsuarioExterno, CancellationToken cancellationToken )
        {
            string str_operacion = "AUTENTICARSE_USUARIO_EXTERNO";
            bool bln_clave_valida = false;
            var respuesta = new ResLoginUsuarioExterno( );
            respuesta.LlenarResHeader(reqLoginUsuarioExterno);
            string password = reqLoginUsuarioExterno.str_password;
            reqLoginUsuarioExterno.str_password = String.Empty;
            await _logsService.SaveHeaderLogs(reqLoginUsuarioExterno, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
            reqLoginUsuarioExterno.str_password = password;

            string token = String.Empty;
            LoginUsrExterno? datosLogin;
            ReqAddKeys reqAddKeys = new( );
            try
            {
                var str_clave_encriptada = await _encrypt.Encrypt(reqLoginUsuarioExterno.str_login, reqLoginUsuarioExterno.str_password, reqLoginUsuarioExterno.str_id_transaccion);

                RespuestaTransaccion res_tran = await _autenticarseUsuariosExternosDat.LoginUsuarioExternoDat(reqLoginUsuarioExterno, str_clave_encriptada);
                respuesta.str_res_codigo = res_tran.codigo;

                if (res_tran.codigo.Equals("000"))
                {
                     
                    datosLogin = Conversions.ConvertConjuntoDatosToClass<LoginUsrExterno>((ConjuntoDatos)res_tran.cuerpo, 0);
                    datosLogin.str_password = datosLogin.str_password!.Trim( );

                    // Validar clave
                    if (!String.IsNullOrEmpty(datosLogin.str_password) || !String.IsNullOrEmpty(datosLogin.str_id_convenio))
                    {
                        string str_concat_clave_usuario = reqLoginUsuarioExterno.str_password + datosLogin.str_id_convenio;
                        bln_clave_valida = Functions.ValidarClave(str_concat_clave_usuario, datosLogin.str_password);

                    }
                    if (bln_clave_valida)
                    {
                        var claims = new ClaimsIdentity(new[]
                            {
                        new Claim( ClaimTypes.Role,  _roles.UsuarioExterno),
                        new Claim( ClaimTypes.NameIdentifier,   datosLogin.str_id_usuario!)
                        });

                        token = await _generarToken.ConstruirToken(reqLoginUsuarioExterno,
                                                                str_operacion,
                                                                claims,
                                                                Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqLoginUsuarioExterno.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                                );
                        respuesta.str_token = token;
                        respuesta.str_res_estado_transaccion = respuesta.str_res_codigo.Equals("000") ? "OK" : res_tran.diccionario["str_error"];
                    }
                    else
                    {
                        reqLoginUsuarioExterno.str_id_usuario = datosLogin.str_id_usuario!;
                        var res_tran_intentos = await _autenticarseUsuariosExternosDat.SetIntentosFallidos(reqLoginUsuarioExterno);
                        respuesta.str_res_codigo = res_tran_intentos.codigo.Equals("1046") ? res_tran_intentos.codigo : "1054";
                        respuesta.str_res_estado_transaccion = res_tran_intentos.diccionario["str_error"];
                        res_tran.diccionario["str_error"] = res_tran_intentos.codigo.Equals("1046") ? res_tran_intentos.diccionario["str_error"] : _parameters.FindErrorCode("1054").str_descripcion;
                    }

                }
                else
                {
                    respuesta.str_res_info_adicional = respuesta.str_res_codigo.Equals("000") ? "OK" : res_tran.diccionario["str_error"];
                    respuesta.str_res_estado_transaccion = respuesta.str_res_codigo.Equals("000") ? "OK" : res_tran.diccionario["str_error"];
                }

                await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
                return respuesta;

            }
            catch (Exception exception)
            {
                await _logsService.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
                throw new ArgumentException(reqLoginUsuarioExterno.str_id_transaccion)!;
            }
        }
    }
}
