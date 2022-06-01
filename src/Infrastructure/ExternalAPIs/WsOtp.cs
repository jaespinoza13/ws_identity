using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;

using Domain.Types;

using Infrastructure.Common.Interfaces;
using Infrastructure.Common.Models;
using Infrastructure.Services;

using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.ExternalAPIs;

public class WsOtp : IWsOtp
{
    private readonly ApiSettings _settings;
    private readonly IHttpService _httpservice;
    private readonly IOtpDat _otpDat;
    public WsOtp ( IOptionsMonitor<ApiSettings> options, IHttpService httpService, IOtpDat otpDat )
    {
        _settings = options.CurrentValue;
        _httpservice = httpService;
        _otpDat = otpDat;
    }

    /// <summary>
    /// Valida si la operacion requiere OTP
    /// </summary>
    /// <returns></returns>
    public async Task<RespuestaTransaccion> ValidaRequiereOtp ( Header header, string str_operacion )
    {
        var cabecera = new
        {

            int_id_sistema = Convert.ToInt32 (header.str_id_sistema),
            int_id_usuario = Convert.ToInt32 (header.str_id_usuario),
            str_usuario = header.str_login,
            int_id_perfil = header.str_id_perfil,
            int_id_oficina = header.str_id_oficina,
            str_nombre_canal = header.str_app,
            str_nemonico_canal = header.str_nemonico_canal,
            str_ip = header.str_ip_dispositivo,
            str_session = header.str_sesion,
            str_mac = header.str_mac_dispositivo
        };

        var cuerpo = new
        {
            str_operacion = str_operacion,
        };

        var raw = new
        {
            cabecera = cabecera,
            cuerpo = cuerpo
        };

        string str_data = JsonSerializer.Serialize (raw);
        RespuestaTransaccion respuesta = await _httpservice.PostRestServiceDataAsync<RespuestaTransaccion> (
                                                    str_data,
                                                    _settings.servicio_ws_otp + "VALIDA_REQUIERE_OTP",
                                                    String.Empty,
                                                    _settings.auth_ws_otp,
                                                    AuthorizationType.BASIC,
                                                    header.str_id_transaccion);
        return respuesta;

    }


    /// <summary>
    /// Valida OTP
    /// </summary>
    /// <returns></returns>
    public async Task<RespuestaTransaccion> ValidaOtp ( dynamic req_valida_otp )
    {

        RespuestaTransaccion res_datos_otp = _otpDat.get_datos_otp (req_valida_otp);
        var datosOtp = Conversions.ConvertConjuntoDatosToClass<ConfiguracionOtp> ((ConjuntoDatos)res_datos_otp.cuerpo);

        var cabecera = new
        {
            int_id_sistema = Convert.ToInt32 (req_valida_otp.str_id_sistema),
            int_id_usuario = Convert.ToInt32 (req_valida_otp.str_id_usuario),
            str_usuario = req_valida_otp.str_login,
            int_id_perfil = req_valida_otp.str_id_perfil,
            int_id_oficina = req_valida_otp.str_id_oficina,
            str_nombre_canal = req_valida_otp.str_app,
            str_nemonico_canal = req_valida_otp.str_nemonico_canal,
            str_ip = req_valida_otp.str_ip_dispositivo,
            str_session = req_valida_otp.str_sesion,
            str_mac = req_valida_otp.str_mac_dispositivo
        };

        var config_otp = new
        {
            int_ente_socio = Convert.ToInt32 (req_valida_otp.str_ente),
            str_celular = datosOtp!.str_celular,
            str_canal = req_valida_otp.str_nemonico_canal,
            str_proceso = req_valida_otp.str_app,
            str_servicio = req_valida_otp.str_id_servicio,
            str_clave = req_valida_otp.str_otp
        };

        var raw = new
        {
            cabecera = cabecera,
            cuerpo = config_otp
        };


        string str_data = JsonSerializer.Serialize (raw);
        RespuestaTransaccion respuesta = await _httpservice.PostRestServiceDataAsync<RespuestaTransaccion>
                                                (str_data,
                                                    _settings.servicio_ws_otp + "VALIDA_OTP",
                                                    String.Empty,
                                                    _settings.auth_ws_otp,
                                                    AuthorizationType.BASIC,
                                                    req_valida_otp.str_id_transaccion);
        return respuesta;
    }

}
