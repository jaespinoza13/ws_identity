﻿using MediatR;

using Application.Common.Behaviours;
using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Jwt;
using System.Reflection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using Application.ParametrosSeguridad;
using Application.Common.Cryptography;
using System.Text.Json;
using Domain.Entities;
using Application.Common.Functions;

namespace Application.LogIn;

public class LoginHandler : IRequestHandler<ReqAutenticarse, ResAutenticarse>
{
    private readonly ILogs _logsService;
    private readonly string str_clase;
    private readonly IAutenticarseDat _autenticarseDat;
    private readonly IGenerarToken _generarToken;
    private readonly IParametersInMemory _parameters;
    private readonly Roles _roles;
    private readonly ApiSettings _settings;



    public LoginHandler ( ILogs logsService,
                            IAutenticarseDat autenticarseDat,
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

    public async Task<ResAutenticarse> Handle ( ReqAutenticarse reqAutenticarse, CancellationToken cancellationToken )
    {
        string str_operacion = "AUTENTICARSE";
        bool bln_clave_valida = false;
        var respuesta = new ResAutenticarse( );
        respuesta.LlenarResHeader(reqAutenticarse);
        string password = reqAutenticarse.str_password;
        reqAutenticarse.str_password = String.Empty;
        await _logsService.SaveHeaderLogs(reqAutenticarse, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);   
        reqAutenticarse.str_password = password;

        string token = String.Empty;
        Login? datosLogin;
        Persona? datosSocio;
        ReqAddKeys reqAddKeys=new();
        try
        {

            RespuestaTransaccion res_tran = await _autenticarseDat.LoginDat(reqAutenticarse);
            respuesta.str_res_codigo = res_tran.codigo;

            if (res_tran.codigo.Equals("000"))
            {
                datosLogin = Conversions.ConvertConjuntoDatosToClass<Login>((ConjuntoDatos)res_tran.cuerpo, 0);
                datosSocio = Conversions.ConvertConjuntoDatosToClass<Persona>((ConjuntoDatos)res_tran.cuerpo, 1);

                datosSocio.str_correo = datosSocio.str_correo?.Length >= 5 ? EnmascararCorreoElectronico(datosSocio.str_correo) : datosSocio.str_correo;
                datosSocio.str_telefono = datosSocio.str_telefono?.Length == 10 ? EnmascararTelefono(datosSocio.str_telefono) : datosSocio.str_telefono;

                datosLogin.str_password = datosLogin.str_password!.Trim( );
                datosLogin.str_password_temp = datosLogin.str_password_temp!.Trim( );

                // Validar clave
                if (!String.IsNullOrEmpty(datosLogin.str_password) || !String.IsNullOrEmpty(datosLogin.str_password_temp))
                {
                    string str_concat_clave_usuario = reqAutenticarse.str_password + datosSocio.str_ente;

                     bln_clave_valida = !String.IsNullOrEmpty(datosLogin.str_password) ?
                                        Functions.ValidarClave(str_concat_clave_usuario, datosLogin.str_password) : Functions.ValidarClave(str_concat_clave_usuario, datosLogin.str_password_temp);

                }
                if (bln_clave_valida)
                {
                    datosSocio.str_id_usuario = datosLogin.str_id_usuario;
                    datosSocio.int_id_perfil = datosLogin.int_id_perfil;
                    var claims = new ClaimsIdentity(new[]
                        {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier,   datosSocio.str_ente!)
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

                    reqAutenticarse.str_id_usuario = datosLogin.str_id_usuario!;
                    var res_tran_intentos=await _autenticarseDat.SetIntentosFallidos(reqAutenticarse);
                    respuesta.str_res_codigo = res_tran_intentos.codigo.Equals("1046") ? res_tran_intentos.codigo: "1054";
                    res_tran.diccionario["str_error"] = res_tran_intentos.codigo.Equals("1046") ? res_tran_intentos.diccionario["str_error"] : _parameters.FindErrorCode("1054").str_descripcion;
                }

            }
            else if (res_tran.codigo.Equals("1068"))
            {
                //Credenciales validadas.    
                datosLogin = Conversions.ConvertConjuntoDatosToClass<Login>((ConjuntoDatos)res_tran.cuerpo, 0);
                datosSocio = Conversions.ConvertConjuntoDatosToClass<Persona>((ConjuntoDatos)res_tran.cuerpo, 1);
                datosSocio.str_id_usuario = datosLogin.str_id_usuario;
                datosSocio.int_id_perfil = datosLogin.int_id_perfil;
                var claims = new ClaimsIdentity(new[]
                       {
                        new Claim( ClaimTypes.Role,  _roles.Socio),
                        new Claim( ClaimTypes.NameIdentifier,   datosSocio.str_ente!),

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

                reqAutenticarse.str_id_usuario = datosLogin.str_id_usuario!;
                var res_tran_intentos = await _autenticarseDat.SetIntentosFallidos(reqAutenticarse);
                respuesta.str_res_codigo = res_tran_intentos.codigo.Equals("1046") ? res_tran_intentos.codigo : "1054";
                res_tran.diccionario["str_error"] = res_tran_intentos.codigo.Equals("1046") ? res_tran_intentos.diccionario["str_error"] : _parameters.FindErrorCode("1054").str_descripcion;

            }
            respuesta.str_token = token;
            if (!String.IsNullOrEmpty(token) && _settings.lst_canales_encriptar.Contains(reqAutenticarse.str_nemonico_canal))
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
            await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
            return respuesta;

        }
        catch (Exception exception)
        {
            await _logsService.SaveExceptionLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase, exception);
            throw new ArgumentException(reqAutenticarse.str_id_transaccion)!;
        }
    }

    static string EnmascararTelefono ( string telefono )
    {

        string primerasTresLetras = telefono.Substring(0, 3);
        string ultimasDosLetras = telefono.Substring(8, 2);
        string enmascarado = $"{primerasTresLetras}*****{ultimasDosLetras}";

        return enmascarado;
    }

    static string EnmascararCorreoElectronico ( string correo )
    {
        int longitud = correo.Length;
        int indiceArroba = correo.IndexOf('@');
        int indicePunto = correo.LastIndexOf('.');
        int numPrimerosAsteriscos = indiceArroba - 4;
        int numSegundosAsteriscos = indicePunto - indiceArroba - 3;

        string primerosDosCaracteres = correo.Substring(0, 2);
        string primerosAsteriscos = new string('*', numPrimerosAsteriscos >= 1 ? numPrimerosAsteriscos : 2);
        string segundosAsteriscos = new string('*', numPrimerosAsteriscos >= 0 ? numSegundosAsteriscos : 0);
        string caracteresAntesArroba = correo.Substring(indiceArroba - 2, 2);
        string caracterDespuesArroba = correo.Substring(indiceArroba + 1, 1);
        string caracterAntesPunto = correo.Substring(indicePunto - 1, 1);
        string caracteresDespuesPunto = correo.Substring(indicePunto + 1);

        string enmascarado = $"{primerosDosCaracteres}{primerosAsteriscos}{caracteresAntesArroba}@{caracterDespuesArroba}{segundosAsteriscos}{caracterAntesPunto}.{caracteresDespuesPunto}";

        return enmascarado;
    }
}


