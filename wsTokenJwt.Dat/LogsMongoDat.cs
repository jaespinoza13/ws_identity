using AccesoDatosGrpcMongo.Neg;
using Grpc.Net.Client;
using System.Text.Json;
using wsTokenJwt.Dto;
using wsTokenJwt.Model;
using static AccesoDatosGrpcMongo.Neg.DALMongo;

namespace wsTokenJwt.Dat
{
    public class LogsMongoDat
    {
        private readonly DALMongoClient objClienteMongo;
        private readonly ServiceSettings _serviceSettings;
        public LogsMongoDat(ServiceSettings _settings)
        {
            this._serviceSettings = _settings;
            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(_settings.delayOutGrpcMongo),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(_settings.timeoutGrpcMongo),
                EnableMultipleHttp2Connections = true
            };

            var canal = GrpcChannel.ForAddress(_settings.servicio_grpc_mongo, new GrpcChannelOptions { HttpHandler = handler });
            objClienteMongo = new DALMongoClient(canal);

        }

        public void GuardarCabeceraMongo(object cabecera)
        {
            var ds = new DatosSolicitud();
            try
            {
                String ser_cabecera = JsonSerializer.Serialize(cabecera);
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_peticiones;
                ds.Filter = String.Empty;
                ds.SolTran = ser_cabecera;

                objClienteMongo.insertar_documento(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en mongo: " + ex.Message);
            }
        }


        public void GuardarRespuestaMongo(dynamic ObjRespPeticion)
        {

            var ds = new DatosSolicitud();
            try
            {
                String ser_cabecera = JsonSerializer.Serialize(ObjRespPeticion);
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_respuesta;
                ds.Filter = String.Empty;
                ds.SolTran = ser_cabecera;
                objClienteMongo.insertar_documento(ds);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en mongo: " + ex.Message);
            }
        }
        public void GuardarExcepcionesMongo(ResComun obj_respuesta, object excepcion)
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_errores;
                ds.Filter = String.Empty;
                ds.SolTran = ser_cabecera;

                objClienteMongo.insertar_documento(ds);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en mongo: " + ex.Message);
            }
        }

        public void GuardarExcepcionesDataBase(Header obj_respuesta, object excepcion)
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_errores_db;
                ds.Filter = String.Empty;
                ds.SolTran = ser_cabecera;

                objClienteMongo.insertar_documento(ds);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en mongo: " + ex.Message);
            }

        }



        public void GuardaErroresHttp(Object any, object excepcion, String str_id_transaccion)
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_errores_http;
                ds.Filter = String.Empty;
                ds.SolTran = ser_cabecera;

                objClienteMongo.insertar_documento(ds);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en mongo: " + ex.Message);
            }
        }

        public void GuardarAmenazasMongo(ValidacionInyeccion obj_respuesta)
        {

            try
            {
                var ds = new DatosSolicitud();
                String ser_cabecera = JsonSerializer.Serialize(obj_respuesta);
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_amenazas;
                ds.Filter = String.Empty;
                ds.SolTran = ser_cabecera;

                objClienteMongo.insertar_documento(ds);
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_peticiones_diarias;
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_peticiones_diarias;
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_promedio_peticiones_diarias;
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_peticiones_diarias;
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
                ds.StrNameBD = _serviceSettings.nombre_base_mongo;
                ds.NombreColeccion = _serviceSettings.coll_promedio_peticiones_diarias;
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
}
