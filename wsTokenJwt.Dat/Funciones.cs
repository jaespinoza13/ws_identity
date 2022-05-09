using AccesoDatosGrpcAse.Neg;
using System.Text.Json;
using wsTokenJwt.Dto;
using wsTokenJwt.Model;

namespace wsTokenJwt.Dat
{
    public static class Funciones
    {
        /// <summary>
        /// Obtiene el conjunto de datos formateados
        /// </summary>
        /// <param name="resultado"></param>
        /// <returns></returns>
        public static ConjuntoDatos ObtenerDatos(DatosRespuesta resultado)
        {
            ConjuntoDatos cd = new();
            var lst_tablas = new List<Tabla>();
            for (int k = 0; k < resultado.ListaTablas.Count; k++)
            {
                var lst_filas = new List<Dto.Fila>();
                for (int i = 0; i < resultado.ListaTablas[k].ListaFilas.Count; i++)
                {
                    Dto.Fila fila = new Dto.Fila();

                    for (int j = 0; j < resultado.ListaTablas[k].ListaFilas[i].ListaColumnas.Count; j++)
                    {

                        fila.nombre_valor.Add(resultado.ListaTablas[k].ListaFilas[i].ListaColumnas[j].NombreCampo, resultado.ListaTablas[k].ListaFilas[i].ListaColumnas[j].Valor);

                    }
                    lst_filas.Add(new Dto.Fila { nombre_valor = fila.nombre_valor });
                }
                lst_tablas.Add(new Tabla { lst_filas = lst_filas });
            }
            cd.lst_tablas = lst_tablas;
            return cd;
        }
        public static void llenar_datos_auditoria_salida(DatosSolicitud ds, Header header)
        {
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_id_transaccion", TipoDato = TipoDato.VarChar, ObjValue = header.str_id_transaccion });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_sistema", TipoDato = TipoDato.Integer, ObjValue = header.str_id_sistema });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = header.str_login });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_perfil", TipoDato = TipoDato.Integer, ObjValue = header.str_id_perfil });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_oficina", TipoDato = TipoDato.Integer, ObjValue = header.str_id_oficina });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nemonico_canal", TipoDato = TipoDato.VarChar, ObjValue = header.str_nemonico_canal });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_ip_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = header.str_ip_dispositivo });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_sesion", TipoDato = TipoDato.VarChar, ObjValue = header.str_sesion });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_mac_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = header.str_mac_dispositivo });

            ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
            ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });
        }
        public static void SaveExcepcionDataBaseSybase(ServiceSettings _serviceSettings, Header transaction, String str_metodo, Exception excepcion, string str_clase)
        {
            try
            {
                var error = new { error = excepcion.ToString(), peticion = transaction };
                InfoLog infoLog = new();
                infoLog.str_clase = str_clase;
                infoLog.str_operacion = transaction.str_id_servicio;
                infoLog.str_objeto = error;
                infoLog.str_metodo = str_metodo;
                infoLog.str_fecha = transaction.dt_fecha_operacion;
                infoLog.str_id_transaccion = transaction.str_id_transaccion;
                infoLog.str_tipo = "e:<";

                new LogsMongoDat(_serviceSettings).GuardarExcepcionesDataBase(transaction, excepcion);

                string rutaArchivo = Directory.GetCurrentDirectory() + _serviceSettings.path_logs_errores_db;

                if (!File.Exists(rutaArchivo))
                {
                    Directory.CreateDirectory(rutaArchivo);
                }

                string fileName = Path.Combine(rutaArchivo, DateTime.Now.ToString("yyyyMMdd") + ".txt");

                using (var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(DateTime.Now.ToString("HHmmssff") + " " + infoLog.str_tipo + JsonSerializer.Serialize(infoLog) + " ");
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar excepción de sybase en mongo: " + ex.ToString());
            }

        }
    }
}
