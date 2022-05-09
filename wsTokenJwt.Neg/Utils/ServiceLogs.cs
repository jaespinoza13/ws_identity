using System.Text.Json;
using wsTokenJwt.Dat;
using wsTokenJwt.Dto;
using wsTokenJwt.Model;

namespace wsConsultas.Neg
{
    public static class ServiceLogs
    {
        private static InfoLog infoLog;
        static ServiceSettings? _serviceSettings;
        private static readonly object objetoBloqueoJson = new();
        public static void Init(ServiceSettings settings)
        {
            _serviceSettings = settings;
            infoLog.str_webservice = settings.nombre_base_mongo!;
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
        public static void SaveHeaderLogs(Header transaction, String str_operacion, String str_metodo, String str_clase)
        {
            infoLog.str_id_transaccion = transaction.str_id_transaccion;
            infoLog.str_clase = str_clase;
            infoLog.str_operacion = str_operacion;
            infoLog.str_objeto = transaction!;
            infoLog.str_metodo = str_metodo;
            infoLog.str_fecha = transaction.dt_fecha_operacion;
            infoLog.str_tipo = "s:<";

            // REGISTRA LOGS DE TEXTO 
            RegistrarTramas(infoLog.str_tipo, infoLog, _serviceSettings!.path_logs_token_jwt);

            // REGISTRA LOGS DE MONGO
            new LogsMongoDat(_serviceSettings!).GuardarCabeceraMongo(transaction!);
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
        public static void SaveResponseLogs(dynamic transaction, String str_operacion, String str_metodo, String str_clase)
        {
            infoLog.str_id_transaccion = transaction.str_id_transaccion;
            infoLog.str_clase = str_clase;
            infoLog.str_operacion = str_operacion;
            infoLog.str_objeto = transaction;
            infoLog.str_metodo = str_metodo;
            infoLog.str_fecha = transaction.dt_fecha_operacion;
            infoLog.str_tipo = "r:>";

            // REGISTRA LOGS DE TEXTO 
            RegistrarTramas(infoLog.str_tipo, infoLog, _serviceSettings!.path_logs_token_jwt);

            // REGISTRA LOGS DE MONGO
            new LogsMongoDat(_serviceSettings!).GuardarRespuestaMongo(transaction);
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
        public static void SaveExceptionLogs(dynamic transaction, String str_operacion, String str_metodo, String str_clase, Object obj_error)
        {
            var objError = new { peticion = transaction, error = obj_error };

            infoLog.str_id_transaccion = transaction.str_id_transaccion;
            infoLog.str_clase = str_clase;
            infoLog.str_operacion = str_operacion;
            infoLog.str_objeto = objError.ToString()!;
            infoLog.str_metodo = str_metodo;
            infoLog.str_fecha = transaction.dt_fecha_operacion;
            infoLog.str_tipo = "e:<";

            // REGISTRA LOGS DE TEXTO 
            RegistrarTramas(infoLog.str_tipo, infoLog, _serviceSettings!.path_logs_errores);

            //REGISTRA LOGS DE MONGO
            new LogsMongoDat(_serviceSettings!).GuardarExcepcionesMongo(transaction, obj_error);
        }

        public static void SaveAmenazasLogs(ValidacionInyeccion validacion, String str_operacion, String str_metodo, String str_clase)
        {
            infoLog.str_clase = str_clase;
            infoLog.str_operacion = str_operacion;
            infoLog.str_objeto = validacion!;
            infoLog.str_metodo = str_metodo;
            infoLog.str_fecha = validacion.dtt_fecha;
            infoLog.str_id_transaccion = validacion.idHeader;
            infoLog.str_tipo = "s:<";

            // REGISTRA LOGS DE TEXTO 
            RegistrarTramas(infoLog.str_tipo, infoLog, _serviceSettings!.path_logs_amenazas);

            //REGISTRA LOGS DE MONGO
            new LogsMongoDat(_serviceSettings!).GuardarAmenazasMongo(validacion!);
        }

        /// <summary>
        /// Guarda cualquier error 
        /// </summary>
        /// <param name="Transaccion"></param>
        /// <param name="str_operacion"></param>
        /// <param name="str_metodo"></param>
        /// <param name="str_clase"></param>
        /// <param name="obj_error"></param>
        /// <returns></returns>
        /// 
        public static void SaveHttpErrorLogs(dynamic transaction, String str_metodo, String str_clase, dynamic obj_error, String str_id_transaccion)
        {
            var objError = new { peticion = transaction, error = obj_error };

            infoLog.str_id_transaccion = str_id_transaccion;
            infoLog.str_clase = str_clase;
            infoLog.str_objeto = objError.ToString()!;
            infoLog.str_metodo = str_metodo;
            infoLog.str_fecha = DateTime.Now;
            infoLog.str_tipo = "e:<";

            //REGISTRA LOGS DE TEXTO 
            RegistrarTramas(infoLog.str_tipo, infoLog, _serviceSettings!.path_logs_errores_http);

            //REGISTRA LOGS DE MONGO
            new LogsMongoDat(_serviceSettings!).GuardaErroresHttp(transaction, obj_error, str_id_transaccion);
        }

        public static void RegistrarTramas(String str_tipo, dynamic obj, string ruta)
        {
            string rutaArchivo = Directory.GetCurrentDirectory() + ruta;
            try
            {
                lock (objetoBloqueoJson)
                {

                    if (!File.Exists(rutaArchivo))
                    {
                        Directory.CreateDirectory(rutaArchivo);
                    }

                    string fileName = Path.Combine(rutaArchivo, DateTime.Now.ToString("yyyyMMdd") + ".txt");

                    using (var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(DateTime.Now.ToString("HHmmssff") + " " + str_tipo + JsonSerializer.Serialize(obj) + " ");
                        }
                        fs.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(String.Empty)!;
            }
        }

    }
}