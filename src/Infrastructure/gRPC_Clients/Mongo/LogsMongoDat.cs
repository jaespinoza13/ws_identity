using AccesoDatosGrpcMongo.Neg;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static AccesoDatosGrpcMongo.Neg.DALMongo;

namespace Infrastructure.gGRPC_Clients.Mongo;

public class LogsMongoDat : IMongoDat
{
    private readonly ApiSettings _settings;
    private readonly DALMongoClient objClienteMongo;
    public LogsMongoDat(IOptionsMonitor<ApiSettings> settings)
    {
        this._settings = settings.CurrentValue;

        var handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(_settings.delayOutGrpcMongo),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(_settings.timeoutGrpcMongo),
            EnableMultipleHttp2Connections = true
        };

        var canal = GrpcChannel.ForAddress(_settings.client_grpc_mongo!, new GrpcChannelOptions { HttpHandler = handler });
        objClienteMongo = new DALMongoClient(canal);

    }

    public async Task GuardarCabeceraMongo(Header cabecera)
    {
        var ds = new DatosSolicitud();
        try
        {
            String ser_cabecera = JsonSerializer.Serialize(cabecera);
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_peticiones;
            ds.Filter = String.Empty;
            ds.SolTran = ser_cabecera;

            await objClienteMongo.insertar_documentoAsync(ds);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }
    }


    public async Task GuardarRespuestaMongo(ResComun ObjRespPeticion)
    {

        var ds = new DatosSolicitud();
        try
        {
            String ser_cabecera = JsonSerializer.Serialize(ObjRespPeticion);
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_respuesta;
            ds.Filter = String.Empty;
            ds.SolTran = ser_cabecera;
            await objClienteMongo.insertar_documentoAsync(ds);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }
    }
    public async Task GuardarExcepcionesMongo(dynamic obj_respuesta, object excepcion)
    {
        var ds = new DatosSolicitud();
        try
        {
            var bjson = new
            {
                idHeader = obj_respuesta.str_id_transaccion,
                str_id_servicio = obj_respuesta.str_id_servicio,
                str_nemonico_canal = obj_respuesta.str_nemonico_canal,
                dt_fecha_operacion = obj_respuesta.dt_fecha_operacion,
                str_ip_dispositivo = obj_respuesta.str_ip_dispositivo,
                str_login = obj_respuesta.str_login,
                str_id_oficina = obj_respuesta.str_id_oficina,
                rsc_res_info_adicional = obj_respuesta.str_res_info_adicional,
                error = excepcion.ToString()
            };

            String ser_cabecera = JsonSerializer.Serialize(bjson);
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_errores;
            ds.Filter = String.Empty;
            ds.SolTran = ser_cabecera;

            await objClienteMongo.insertar_documentoAsync(ds);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }
    }

    public async Task GuardarExcepcionesDataBase(dynamic obj_respuesta, object excepcion)
    {
        var ds = new DatosSolicitud();
        try
        {
            var bjson = new
            {
                idHeader = obj_respuesta.str_id_transaccion,
                str_id_servicio = obj_respuesta.str_id_servicio,
                str_nemonico_canal = obj_respuesta.str_nemonico_canal,
                dt_fecha_operacion = obj_respuesta.dt_fecha_operacion,
                str_ip_dispositivo = obj_respuesta.str_ip_dispositivo,
                str_login = obj_respuesta.str_login,
                str_id_oficina = obj_respuesta.str_id_oficina,
                error = excepcion.ToString()
            };

            String ser_cabecera = JsonSerializer.Serialize(bjson);
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_errores_db;
            ds.Filter = String.Empty;
            ds.SolTran = ser_cabecera;

            await objClienteMongo.insertar_documentoAsync(ds);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }

    }

    public async Task GuardaErroresHttp(Object any, object excepcion, String str_id_transaccion)
    {
        var ds = new DatosSolicitud();
        try
        {
            var bjson = new
            {
                str_id_transaccion = str_id_transaccion,
                fecha = DateTime.Now,
                objeto = any,
                error = excepcion.ToString(),
            };

            String ser_cabecera = JsonSerializer.Serialize(bjson);
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_errores_http;
            ds.Filter = String.Empty;
            ds.SolTran = ser_cabecera;

            await objClienteMongo.insertar_documentoAsync(ds);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }
    }

    public async Task GuardarAmenazasMongo(ValidacionInyeccion obj_respuesta)
    {

        try
        {
            var ds = new DatosSolicitud();
            String ser_cabecera = JsonSerializer.Serialize(obj_respuesta);
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_amenazas;
            ds.Filter = String.Empty;
            ds.SolTran = ser_cabecera;

            await objClienteMongo.insertar_documentoAsync(ds);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);

        }
    }

    public RespuestaTransaccion buscar_peticiones_diarias(string filtro)
    {
        var respuesta = new RespuestaTransaccion();
        var ds = new DatosSolicitud();
        try
        {
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_peticiones_diarias;
            ds.Filter = filtro;
            ds.SolTran = String.Empty;

            DatosRespuesta res = objClienteMongo.buscar_documentos(ds);

            respuesta.codigo = "000";
            respuesta.cuerpo = res.Mensaje;
        }
        catch (Exception ex)

        {
            respuesta.codigo = "001";
            respuesta.diccionario.Add("str_error", ex.ToString());
        }
        return respuesta;
    }

    public void actualizar_peticion_diaria(string filtro, string peticion)
    {
        var ds = new DatosSolicitud();

        try
        {
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_peticiones_diarias;
            ds.Filter = filtro;
            ds.SolTran = peticion;
            objClienteMongo.actualizar_documento_avanzado(ds);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }

    }

    public void guardar_promedio_peticion_diaria(string str_operacion, string str_fecha)
    {

        var ds = new DatosSolicitud();
        try
        {
            string str_filtro = "{'str_operacion':'" + str_operacion + "'}";
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_promedio_peticiones_diarias;
            ds.Filter = str_filtro;
            ds.SolTran = String.Empty;
            DatosRespuesta res = objClienteMongo.buscar_documentos(ds);
            var resp_mongo = res.Mensaje;
            int promedio = calcular_promedio(str_operacion);
            if (resp_mongo != null && resp_mongo.ToString() != "[]")
            {
                var res_datos_mongo = resp_mongo.ToString()!.Replace("ObjectId(", " ").Replace(")", " ");
                res_datos_mongo = res_datos_mongo.Replace("[", "").Replace("]", "");
                var prom_peticion_diaria = JsonSerializer.Deserialize<PromedioPeticionDiaria>(res_datos_mongo);

                if (prom_peticion_diaria!._id != null)
                {
                    string str_datos_update = "{$set:{'dbl_promedio_peticion':" + promedio + ",'str_fecha_actualizacion':'" + str_fecha + "'}}";

                    ds.Filter = str_filtro;
                    ds.SolTran = str_datos_update;

                    objClienteMongo.actualizar_documento(ds);
                }
            }
            else
            {
                object obj_sol = new { dbl_promedio_peticion = promedio, str_operacion, str_fecha_actualizacion = str_fecha };
                ds.Filter = String.Empty;
                ds.SolTran = JsonSerializer.Serialize(obj_sol);
                objClienteMongo.insertar_documento(ds);

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en mongo: " + ex.Message);
        }
    }

    public int calcular_promedio(string str_operacion)
    {
        string str_filtro = "[{ $match: { str_operacion:'" + str_operacion + "'} }," +
            "{$group:{_id: '$str_operacion',dbl_promedio_peticion: { $avg: '$int_num_peticion'}}}]";
        int int_respuesta = 0;
        var ds = new DatosSolicitud();
        try
        {
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_peticiones_diarias;
            ds.Filter = str_filtro;
            ds.SolTran = String.Empty;

            DatosRespuesta res = objClienteMongo.buscar_documentos_avanzado(ds);

            string res_datos_mongo = res.Mensaje;
            if (res_datos_mongo != null && res_datos_mongo.ToString() != "[]")
            {
                res_datos_mongo = res_datos_mongo.Replace("[", "").Replace("]", "");
                var peticion_diaria = JsonSerializer.Deserialize<PromedioPeticionDiaria>(res_datos_mongo);
                int_respuesta = Convert.ToInt32(peticion_diaria!.dbl_promedio_peticion);

            }

        }
        catch (Exception)

        {
            int_respuesta = 0;

        }
        return int_respuesta;
    }
    public int obtener_promedio(string str_operacion)
    {
        string str_filtro = "{'str_operacion':'" + str_operacion + "'}";
        int int_respuesta = 0;
        var ds = new DatosSolicitud();
        try
        {
            ds.StrNameBD = _settings.nombre_base_mongo;
            ds.NombreColeccion = _settings.coll_promedio_peticiones_diarias;
            ds.Filter = str_filtro;
            ds.SolTran = String.Empty;

            DatosRespuesta res = objClienteMongo.buscar_documentos(ds);

            string res_datos_mongo = res.Mensaje;
            if (res_datos_mongo != null && res_datos_mongo.ToString() != "[]")
            {
                res_datos_mongo = res_datos_mongo.ToString()!.Replace("ObjectId(", " ").Replace(")", " ");

                res_datos_mongo = res_datos_mongo.Replace("[", "").Replace("]", "");
                var peticion_diaria = JsonSerializer.Deserialize<PromedioPeticionDiaria>(res_datos_mongo);
                int_respuesta = Convert.ToInt32(peticion_diaria!.dbl_promedio_peticion);

            }

        }
        catch (Exception)

        {
            int_respuesta = 0;

        }
        return int_respuesta;
    }
}