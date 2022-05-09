using System.Reflection;
using System.Text.Json;
using wsConsultas.Neg;
using wsTokenJwt.Dat;
using wsTokenJwt.Dto;
using wsTokenJwt.Model;

namespace wsTokenJwt.Neg.Utils
{
    public static class Utils
    {

        #region Métodos "Conversión de Conjunto de Datos a un objeto/Lista de una Clase"
        /// <summary>
        /// Convierte un Conjunto de datos a una lista de una Clase específica
        /// </summary>
        /// <param name="conjuntoDatos"></param>
        /// <returns></returns>
        public static List<T> ConvertConjuntoDatosToListClass<T>(ConjuntoDatos conjuntoDatos)
        {
            var lst_array = new List<T>();

            foreach (var item in conjuntoDatos.lst_tablas[0].lst_filas)
            {
                T obj = (T)Converting.MapDictToObj(item.nombre_valor, typeof(T));
                lst_array.Add(obj);
            }

            return lst_array;
        }


        /// <summary>
        /// Convierte un Conjunto de datos a un objeto de una Clase específica
        /// </summary>
        /// <param name="cuerpo"></param>
        /// <returns></returns>
        public static T? ConvertConjuntoDatosToClass<T>(ConjuntoDatos conjuntoDatos)
        {
            T? obj = default(T);
            foreach (var item in conjuntoDatos.lst_tablas[0].lst_filas)
            {
                obj = (T)Converting.MapDictToObj(item.nombre_valor, typeof(T));
            }

            return obj;
        }
        #endregion

        #region Método "Generar número aleatorio"
        /// <summary>
        /// Genera un número aleatorio en string
        /// </summary>
        /// <returns></returns>
        internal static string GeneraCadenaAleatoria()
        {
            Random random = new Random();
            const string characters = "0123456789";
            return new string(Enumerable.Repeat(characters, 20)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        #region Método "Validar Token de Acceso"
        /// <summary>
        /// Valida token de acceso
        /// </summary>
        /// <returns></returns>
        public async static Task<ResComun> ValidarToken(ServiceSettings settings, string str_data)
        {
            string parametros = settings.prm_ws_acceso + "VALIDAR_TOKEN";
            var service = new ServiceHttp<ResComun>();
            ResComun respuesta = await service.PostRestServiceDataAsync(str_data, settings.servicio_ws_token_jwt, parametros, settings.auth_ws_token_jwt, settings);
            return respuesta;

        }
        #endregion

        #region Método para validar petición
        public async static Task<ResComun> RequestControl(ServiceSettings settings, Header raw, string str_operacion)
        {
            ResComun respuesta = new();

            try
            {
                var str_canal = LoadConfigService.FindParametro(raw.str_nemonico_canal).str_nemonico;

                var bl_validar = settings.lst_canales_valida_token!.Contains(str_canal);

                // CONTROL DEL PETICIONES DIARIAS
                control_peticion_diaria(str_operacion, settings, respuesta);

                if (bl_validar)
                {
                    //VALIDACIÓN DE TOKEN
                    respuesta = await ValidarToken(settings, JsonSerializer.Serialize(raw));
                }

                respuesta.str_res_codigo = bl_validar ? respuesta.str_res_codigo : "000";
                respuesta.str_res_estado_transaccion = bl_validar ? respuesta.str_res_estado_transaccion : "OK";
                respuesta.str_res_info_adicional = LoadConfigService.FindErrorCode(respuesta.str_res_codigo)!.str_valor_fin;
                return respuesta;
            }
            catch (Exception)
            {
                throw new Exception(raw.str_nemonico_canal + " CANAL INVÁLIDO")!;
            }
        }
        #endregion

        #region"Control de peticiones diarias"
        public static bool control_peticion_diaria(string str_operacion, ServiceSettings serviceSettings, ResComun respuestaLog)
        {

            bool respuesta = true;

            string str_fecha_diaria = DateTime.Now.ToString("yyyy-MM-dd");
            string str_filtro = "{'str_fecha_solicitud':'" + str_fecha_diaria + "','str_operacion':'" + str_operacion + "'}";

            try
            {
                RespuestaTransaccion var_respuesta = new LogsMongoDat(serviceSettings!).buscar_peticiones_diarias(str_filtro);
                int int_act_peticiones = 1;
                if (var_respuesta.codigo == "000")
                {
                    var resp_mongo = var_respuesta.cuerpo;
                    if (resp_mongo != null && resp_mongo.ToString() != "[]")
                    {
                        var res_datos_mongo = var_respuesta.cuerpo.ToString()!.Replace("ObjectId(", " ").Replace(")", " ");
                        res_datos_mongo = res_datos_mongo.Replace("[", "").Replace("]", "");
                        PeticionDiaria peticion_diaria = JsonSerializer.Deserialize<PeticionDiaria>(res_datos_mongo)!;
                        if (peticion_diaria!._id != null)
                        {
                            int_act_peticiones = peticion_diaria.int_num_peticion + 1;

                            int respuesta_promedio = new LogsMongoDat(serviceSettings!).obtener_promedio(str_operacion);
                            var cantidad_maxima = respuesta_promedio * Convert.ToInt32(LoadConfigService.FindParametro("PRM_MAXIMO_PETICIONES_DIARIAS")!.str_valor_ini) / 100;

                            if (serviceSettings.valida_peticiones_diarias && int_act_peticiones > cantidad_maxima)
                                respuesta = false;

                        }
                    }
                    else
                    {
                        new LogsMongoDat(serviceSettings!).guardar_promedio_peticion_diaria(str_operacion, str_fecha_diaria);
                    }

                    string str_act_registro = "{$set:{'int_num_peticion':" + int_act_peticiones + ", 'str_fecha_solicitud' : '" + str_fecha_diaria + "',str_operacion:'" + str_operacion + "'}}";
                    new LogsMongoDat(serviceSettings!).actualizar_peticion_diaria(str_filtro, str_act_registro);


                }
            }
            catch (Exception exception)
            {
                ServiceLogs.SaveExceptionLogs(respuestaLog, str_operacion, MethodBase.GetCurrentMethod()!.Name, "Utils", exception);

                respuesta = true;

            }
            return respuesta;
        }
        #endregion
    }
}
