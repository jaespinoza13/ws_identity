
using AccesoDatosGrpcAse.Neg;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Grpc.Net.Client;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.Common.Funciones
{
    internal static class Funciones
    {
        public static ConjuntoDatos ObtenerDatos(DatosRespuesta resultado)
        {
            ConjuntoDatos cd = new();
            var lst_tablas = new List<Tabla>();
            for (int k = 0; k < resultado.ListaTablas.Count; k++)
            {
                var lst_filas = new List<Application.Common.Models.Fila>();
                for (int i = 0; i < resultado.ListaTablas[k].ListaFilas.Count; i++)
                {
                    Application.Common.Models.Fila fila = new();

                    for (int j = 0; j < resultado.ListaTablas[k].ListaFilas[i].ListaColumnas.Count; j++)
                    {

                        fila.nombre_valor.Add(resultado.ListaTablas[k].ListaFilas[i].ListaColumnas[j].NombreCampo, resultado.ListaTablas[k].ListaFilas[i].ListaColumnas[j].Valor);

                    }
                    lst_filas.Add(new Application.Common.Models.Fila { nombre_valor = fila.nombre_valor });
                }
                lst_tablas.Add(new Tabla { lst_filas = lst_filas });
            }
            cd.lst_tablas = lst_tablas;
            return cd;
        }

        public static void LlenarDatosAuditoria(DatosSolicitud ds, Header header)
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
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_pais", TipoDato = TipoDato.VarChar, ObjValue = header.str_pais });

            ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
            ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });
        }

        /// <summary>
        /// Abrir conexión hacia el GRPC
        /// </summary>
        /// <param name="str_url_grcp"></param>
        /// <returns></returns>

        public static (GrpcChannel, DALClient) getConnection ( string str_url_grcp )
        {
            var httpHandler = new HttpClientHandler( );
            var grpcChannel = GrpcChannel.ForAddress(str_url_grcp, new GrpcChannelOptions { HttpHandler = httpHandler });
            return (grpcChannel, new DALClient(grpcChannel));
        }

        /// <summary>
        /// Cerrar conexión grpc
        /// </summary>
        /// <param name="grpcChannel"></param>
        public static void setCloseConnection ( GrpcChannel grpcChannel )
        {
            grpcChannel.Dispose( );
        }
    }
}
