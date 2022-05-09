using AccesoDatosGrpcAse.Neg;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using wsTokenJwt.Dto;
using wsTokenJwt.Model;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace wsTokenJwt.Dat
{
    public class UtilidadesParametrosDat
    {
        private readonly ServiceSettings _settings;
        private readonly DALClient objClienteDal;
        private readonly string str_clase;

        public UtilidadesParametrosDat(ServiceSettings serviceSettings)
        {
            _settings = serviceSettings;
            this.str_clase = GetType().FullName!;

            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(serviceSettings.delayOutGrpcSybase),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(serviceSettings.timeoutGrpcSybase),
                EnableMultipleHttp2Connections = true
            };

            var canal = GrpcChannel.ForAddress(_settings.servicio_grpc_sybase, new GrpcChannelOptions { HttpHandler = handler });
            objClienteDal = new DALClient(canal);
        }


        public RespuestaTransaccion get_parametros(ReqGetParametros req_get_parametros)
        {
            var respuesta = new RespuestaTransaccion();

            try
            {
                DatosSolicitud ds = new();

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nombre", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_nombre.ToString() });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nemonico", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_nemonico.ToString() });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_front", TipoDato = TipoDato.Integer, ObjValue = req_get_parametros.int_front.ToString() });

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_id_transaccion", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_id_transaccion });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_sistema", TipoDato = TipoDato.Integer, ObjValue = req_get_parametros.str_id_sistema });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_login });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nemonico_canal", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_nemonico_canal });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_ip_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_ip_dispositivo });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_sesion", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_sesion });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_mac_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_mac_dispositivo });

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });


                ds.NombreSP = "get_parametros";
                ds.NombreBD = _settings.BD_meg_megonline;

                var resultado = objClienteDal.ExecuteDataSet(ds);
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);
                var str_codigo = lst_valores.Find(x => x.StrNameParameter == "@int_o_error_cod")!.ObjValue;
                var str_error = lst_valores.Find(x => x.StrNameParameter == "@str_o_error")!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft(3, '0');
                respuesta.cuerpo = Funciones.ObtenerDatos(resultado);
                respuesta.diccionario.Add("str_error", str_error.ToString());
            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add("str_error", exception.ToString());
                Funciones.SaveExcepcionDataBaseSybase(_settings, req_get_parametros, MethodBase.GetCurrentMethod()!.Name, exception, str_clase);
                throw new Exception(req_get_parametros.str_id_transaccion)!;

            }
            return respuesta;
        }

        public RespuestaTransaccion get_datos_otp(dynamic req_get_datos)
        {
            var respuesta = new RespuestaTransaccion();

            try
            {
                DatosSolicitud ds = new DatosSolicitud();

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_ente", TipoDato = TipoDato.Integer, ObjValue = req_get_datos.str_ente });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_proceso", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_servicio });

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_sistema", TipoDato = TipoDato.Integer, ObjValue = req_get_datos.str_id_sistema });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_canal", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_nemonico_canal });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_login });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_equipo", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_ip_dispositivo });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_sesion", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_sesion });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_mac", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_mac_dispositivo });

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@o_error_cod", TipoDato = TipoDato.Integer });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@o_error", TipoDato = TipoDato.VarChar });

                ds.NombreSP = "get_datos_otp_gen";
                ds.NombreBD = _settings.BD_megservicios;

                var resultado = objClienteDal.ExecuteDataSet(ds);
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);
                var str_codigo = lst_valores.Find(x => x.StrNameParameter == "@o_error_cod")!.ObjValue;
                var str_error = lst_valores.Find(x => x.StrNameParameter == "@o_error")!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft(3, '0');
                respuesta.cuerpo = Funciones.ObtenerDatos(resultado);
                respuesta.diccionario.Add("str_error", str_error.ToString());
            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add("str_error", exception.ToString());
                Funciones.SaveExcepcionDataBaseSybase(_settings, req_get_datos, MethodBase.GetCurrentMethod()!.Name, exception, str_clase);
                throw new Exception(req_get_datos.str_id_transaccion)!;

            }
            return respuesta;
        }

        public RespuestaTransaccion get_montos(Header req_get_montos)
        {
            var respuesta = new RespuestaTransaccion();

            try
            {
                DatosSolicitud ds = new();
                Funciones.llenar_datos_auditoria_salida(ds, req_get_montos);
                ds.NombreSP = "get_montos";
                ds.NombreBD = _settings.BD_megservicios;

                var resultado = objClienteDal.ExecuteDataSet(ds);
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);
                var str_codigo = lst_valores.Find(x => x.StrNameParameter == "@int_o_error_cod")!.ObjValue;
                var str_error = lst_valores.Find(x => x.StrNameParameter == "@str_o_error")!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft(3, '0');
                respuesta.cuerpo = Funciones.ObtenerDatos(resultado);
                respuesta.diccionario.Add("str_error", str_error.ToString());
            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add("str_error", exception.ToString());
                Funciones.SaveExcepcionDataBaseSybase(_settings, req_get_montos, MethodBase.GetCurrentMethod()!.Name, exception, str_clase);
                throw new Exception(req_get_montos.str_id_transaccion)!;

            }
            return respuesta;
        }
    }
}
