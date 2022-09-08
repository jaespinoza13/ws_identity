using MediatR;

using Application.Common.Behaviours;
using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using Domain.Models;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using Application.ParametrosSeguridad;
using Application.Common.Cryptography;
using System.Text.Json;
using static Application.Common.Cryptography.CryptographyRSA;

namespace Application.LogIn;

public class LoginHandler : IRequestHandler<ReqAutenticarse, ResAutenticarse>
{
    private readonly ILogs _logsService;
    private readonly string str_clase;
    private readonly IAutenticarseDat _autenticarseDat;
    private readonly IEncryptMego _encrypt;
    private readonly IGenerarToken _generarToken;
    private readonly IParametersInMemory _parameters;
    private readonly Roles _roles;
    private readonly IMemoryCache _memoryCache;
    private readonly ApiSettings _settings;



    public LoginHandler ( ILogs logsService,
                            IAutenticarseDat autenticarseDat,
                            IEncryptMego encrypt,
                            IGenerarToken generarToken,
                            IParametersInMemory parameters,
                            IOptionsMonitor<Roles> roles,
                              IOptionsMonitor<ApiSettings> options,
                             IMemoryCache memoryCache
                         )
    {
        _logsService = logsService;
        str_clase = GetType( ).FullName!;
        _autenticarseDat = autenticarseDat;
        _encrypt = encrypt;
        _generarToken = generarToken;
        _parameters = parameters;
        _memoryCache = memoryCache;
        _settings = options.CurrentValue;
        _roles = roles.CurrentValue;
    }

    public async Task<ResAutenticarse> Handle ( ReqAutenticarse reqAutenticarse, CancellationToken cancellationToken )
    {
        string str_operacion = "AUTENTICARSE";
        bool bln_clave_valida = false;
        var respuesta = new ResAutenticarse( );
        respuesta.LlenarResHeader(reqAutenticarse);
        string password = reqAutenticarse.str_password;
        reqAutenticarse.str_password = String.Empty;
        await _logsService.SaveHeaderLogs(reqAutenticarse, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
        reqAutenticarse.str_password = password;

        string token = String.Empty;
        Login? datosLogin;
        Persona? datosSocio;
        ReqAddKeys reqAddKeys=new();
        try
        {
            var str_clave_encriptada = await _encrypt.Encrypt(reqAutenticarse.str_login, reqAutenticarse.str_password, reqAutenticarse.str_id_transaccion);

            RespuestaTransaccion res_tran = await _autenticarseDat.LoginDat(reqAutenticarse, str_clave_encriptada);
            respuesta.str_res_codigo = res_tran.codigo;

            if (res_tran.codigo.Equals("000"))
            {

                datosLogin = Conversions.ConvertConjuntoDatosToClass<Login>((ConjuntoDatos)res_tran.cuerpo, 0);
                datosSocio = Conversions.ConvertConjuntoDatosToClass<Persona>((ConjuntoDatos)res_tran.cuerpo, 1);

                datosLogin.str_password = datosLogin.str_password!.Trim( );
                datosLogin.str_password_temp = datosLogin.str_password_temp!.Trim( );

                // Validar clave
                if (!String.IsNullOrEmpty(datosLogin.str_password) || !String.IsNullOrEmpty(datosLogin.str_password_temp))
                {
                    string str_concat_clave_usuario = reqAutenticarse.str_password + datosSocio.int_ente;

                    bln_clave_valida = !String.IsNullOrEmpty(datosLogin.str_password) ?
                                        ValidarClave(str_concat_clave_usuario, datosLogin.str_password) : ValidarClave(str_concat_clave_usuario, datosLogin.str_password_temp);

                }
                if (bln_clave_valida)
                {
                    //BORRAR
                    datosSocio.int_id_usuario = datosLogin.int_id_usuario;
                    //BORRAR
                    datosSocio.str_id_usuario = datosLogin.str_id_usuario;
                    datosSocio.int_id_perfil = datosLogin.int_id_perfil;
                    var claims = new ClaimsIdentity(new[]
                        {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier,   datosSocio.int_ente.ToString())
                        });

                    token = await _generarToken.ConstruirToken(reqAutenticarse,
                                                            str_operacion,
                                                            claims,
                                                            Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqAutenticarse.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                            );
                    respuesta.objSocio = datosSocio;
                }
                else
                {
                    //Clave incorrecta
                    respuesta.str_res_codigo = "1054";
                    res_tran.diccionario["str_error"] = _parameters.FindErrorCode("1054").str_descripcion;
                    reqAutenticarse.str_id_usuario = datosLogin.int_id_usuario.ToString( );
                    await _autenticarseDat.SetIntentosFallidos(reqAutenticarse);
                }

            }
            else if (res_tran.codigo.Equals("1068"))
            {
                //Credenciales validadas.    
                datosLogin = Conversions.ConvertConjuntoDatosToClass<Login>((ConjuntoDatos)res_tran.cuerpo, 0);
                datosSocio = Conversions.ConvertConjuntoDatosToClass<Persona>((ConjuntoDatos)res_tran.cuerpo, 1);
                datosSocio.int_id_usuario = datosLogin.int_id_usuario;
                datosSocio.int_id_perfil = datosLogin.int_id_perfil;
                var claims = new ClaimsIdentity(new[]
                       {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier,   datosSocio.int_ente.ToString()),

                        });
                token = await _generarToken.ConstruirToken(reqAutenticarse,
                                                            str_operacion,
                                                            claims,
                                                            Convert.ToDouble(_parameters.FindParametro("TIEMPO_MAXIMO_TOKEN_" + reqAutenticarse.str_nemonico_canal.ToUpper( )).str_valor_ini)
                                                            );
                respuesta.objSocio = datosSocio;
                respuesta.str_res_codigo = "000";

            }
            else if (res_tran.codigo.Equals("1054"))
            {
                datosLogin = Conversions.ConvertConjuntoDatosToClass<Login>((ConjuntoDatos)res_tran.cuerpo, 0);

                //Clave incorrecta
                respuesta.str_res_codigo = "1054";
                reqAutenticarse.str_id_usuario = datosLogin.int_id_usuario.ToString( );
                await _autenticarseDat.SetIntentosFallidos(reqAutenticarse);
            }
            respuesta.str_token = token;
            if (!String.IsNullOrEmpty(token))
            {
                var KeyCreate = CryptographyRSA.GenerarLlavePublicaPrivada();
                var ClaveSecreta = Guid.NewGuid( ).ToString( );
                reqAddKeys = JsonSerializer.Deserialize<ReqAddKeys>(JsonSerializer.Serialize(reqAutenticarse))!;

                respuesta.objSocio!.str_mod = KeyCreate.str_modulo;
                respuesta.objSocio.str_exp = KeyCreate.str_exponente;
                respuesta.str_clave_secreta = ClaveSecreta;

                reqAddKeys.str_ente = respuesta.objSocio.str_ente!;
                reqAddKeys.str_modulo = KeyCreate.str_modulo!;
                reqAddKeys.str_exponente = KeyCreate.str_exponente!;
                reqAddKeys.str_clave_secreta = ClaveSecreta!;
                reqAddKeys.str_llave_privada = KeyCreate.str_xml_priv!;
                reqAddKeys.str_llave_simetrica = CryptographyAES.GenerarLlaveHexadecimal(16);
                res_tran = await _autenticarseDat.AddKeys(reqAddKeys);

            }
            respuesta.str_res_estado_transaccion = respuesta.str_res_codigo.Equals("000") ? "OK" : "ERR";
            respuesta.str_res_info_adicional = res_tran.diccionario["str_error"].ToString( );
            await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase);
            return respuesta;

        }
        catch (Exception exception)
        {
            await _logsService.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod( )!.Name, str_clase, exception);
            throw new ArgumentException(reqAutenticarse.str_id_transaccion)!;
        }
    }
    public static bool ValidarClave ( string claveUsuario, string claveBase )
    {
        return BCrypt.Net.BCrypt.Verify(claveUsuario, claveBase);
    }
}


