using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;

using Infrastructure.Common.Tramas;
using Infrastructure.gGRPC_Clients.Mongo;

using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Services;

public class LogsService : ILogs
{
    private readonly InfoLog infoLog = new( );
    private readonly ApiSettings _settings;
    private readonly IMongoDat _mongoDat;
    private readonly IHttpService _httpService;
    public LogsService ( IOptionsMonitor<ApiSettings> options, IMongoDat mongoDat, IHttpService httpService )
    {
        this._settings = options.CurrentValue;
        this.infoLog.str_webservice = _settings.nombre_base_mongo;
        this._mongoDat = mongoDat;
        this._httpService = httpService;
    }

    /// <summary>
    /// Guarda en mongodb la cabecera de la petición
    /// </summary>
    /// <param name="Transaccion"></param>
    /// <param name="str_operacion"></param>
    /// <param name="str_metodo"></param>
    /// <param name="str_clase"></param>
    /// <returns></returns>
    /// 
    public Task SaveHeaderLogs ( dynamic transaction, String str_operacion, String str_metodo, String str_clase )
    {

        getArmarObjeto("SOLICITUD", transaction, str_operacion, str_metodo, str_clase, "");
        return Task.CompletedTask;

    }

    /// <summary> 
    /// Guarda en mongodb la respuesta de la petición
    /// </summary>
    /// <param name="Transaccion"></param>
    /// <param name="str_operacion"></param>
    /// <param name="str_metodo"></param>
    /// <param name="str_clase"></param>
    /// <returns></returns>
    /// 
    public  Task SaveResponseLogs ( dynamic transaction, String str_operacion, String str_metodo, String str_clase )
    {

        transaction.dt_fecha_operacion = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        getArmarObjeto("RESPUESTA", transaction, str_operacion, str_metodo, str_clase, "");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Guarda exepciones de codigo
    /// </summary>
    /// <param name="Transaccion"></param>
    /// <param name="str_operacion"></param>
    /// <param name="str_metodo"></param>
    /// <param name="str_clase"></param>
    /// <param name="obj_error"></param>
    /// <returns></returns>
    /// 
    public Task SaveExceptionLogs ( dynamic transaction, String str_operacion, String str_metodo, String str_clase, Exception obj_error )
    {

        transaction.dt_fecha_operacion = DateTime.Now;
        transaction.str_res_estado_transaccion = "ERR";
        transaction.str_res_codigo = "001";

        getArmarObjeto("RESPUESTA", transaction, str_operacion, str_metodo, str_clase, "CODE", obj_error.ToString( ));

        return Task.CompletedTask;
    }

    public Task SaveAmenazasLogs ( ValidacionInyeccion validacion, String str_operacion, String str_metodo, String str_clase )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Guarda cualquier error 
    /// </summary>
    /// <param name="Transaccion"></param>
    /// <param name="str_operacion"></param>
    /// <param name="str_metodo"></param>
    /// <param name="str_clase"></param>
    /// <param name="obj_error"></param>
    /// <param name="str_id_transaccion"></param>
    /// <returns></returns>
    /// 
    public Task SaveHttpErrorLogs ( dynamic transaction, String str_metodo, String str_clase, dynamic obj_error, String str_id_transaccion )
    {
        return Task.CompletedTask;
    }


    /// <summary>
    /// Guardar errores de base
    /// </summary>
    /// <param name="obj_error"></param>
    /// <returns></returns>
    public Task SaveExcepcionDataBaseSybase ( dynamic transaction, string str_operacion, string str_metodo, string str_clase, Exception obj_error )
    {

        try
        {
            var respuesta = new ResComun( );
            respuesta.LlenarResHeader(transaction);
            respuesta.str_res_estado_transaccion = "ERR";
            respuesta.str_res_codigo = "003";
            getArmarObjeto("RESPUESTA", respuesta, str_operacion, str_metodo, str_clase, "BD", obj_error.ToString( ));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en base de datos: " + ex.Message);
        }
        return Task.CompletedTask;
    }


    public void getArmarObjeto ( string str_tipo, dynamic dyn_objeto, string str_operacion, string str_metodo, string str_clase, string str_tipo_error, string str_error = "" )
    {
        try
        {
            Dictionary<string, object> dic_obj = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(dyn_objeto));

            dic_obj.Add("str_tipo_error", str_tipo_error);
            dic_obj.Add("str_error", str_error);

            var solicitud = new
            {
                dtt_fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                str_id_transaccion = dyn_objeto.str_id_transaccion,
                str_tipo = str_tipo,
                str_webservice = _settings.nombre_base_mongo,
                str_clase = str_clase,
                str_metodo = str_metodo,
                str_operacion = str_operacion,
                obj_objeto = dic_obj,
                bln_save_log_txt = true,
                str_nombre_coleccion = "",
                str_nombre_bd = ""
            };

            var solServicio = new SolicitarServicio( );

            solServicio.objSolicitud = solicitud;
            solServicio.urlServicio = _settings.url_logs + "ADD_LOG";
            solServicio.tipoAuth = "Authorization-Mego";
            solServicio.valueAuth = _settings.auth_logs;

            _httpService.solicitar_servicio_async(solServicio);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString( ));
        }

    }
}
