using AccesoDatosGrpcAse.Neg;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.LogIn;
using Grpc.Net.Client;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
    internal class AutenticarseDat : IAutenticarseDat
    {
        private readonly ApiSettings _settings;
        private readonly DALClient objClienteDal;
        private readonly ILogs _logsService;
        private readonly string str_clase;

        public AutenticarseDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService )
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType( ).FullName!;

            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(_settings.delayOutGrpcSybase),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(_settings.timeoutGrpcSybase),
                EnableMultipleHttp2Connections = true
            };

            var canal = GrpcChannel.ForAddress(_settings.client_grpc_sybase!, new GrpcChannelOptions { HttpHandler = handler });
            objClienteDal = new DALClient(canal);
        }


        public async Task<RespuestaTransaccion> LoginDat ( ReqAutenticarse reqAutenticarse, string claveEncriptada )
        {
            var respuesta = new RespuestaTransaccion( );

            try
            {
                DatosSolicitud ds = new( );
                Funciones.LlenarDatosAuditoria(ds, reqAutenticarse);
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_clave", TipoDato = TipoDato.VarChar, ObjValue = claveEncriptada });

                ds.NombreSP = "get_login_autenticar";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = await objClienteDal.ExecuteDataSetAsync(ds);
                var lst_valores = new List<ParametroSalidaValores>( );

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);
                var str_codigo = lst_valores.Find(x => x.StrNameParameter == "@int_o_error_cod")!.ObjValue;
                var str_error = lst_valores.Find(x => x.StrNameParameter == "@str_o_error")!.ObjValue.Trim( );

                respuesta.codigo = str_codigo.ToString( ).Trim( ).PadLeft(3, '0');
                respuesta.cuerpo = Funciones.ObtenerDatos(resultado);
                respuesta.diccionario.Add("str_error", str_error.ToString( ));

            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add("str_error", exception.ToString( ));
                await _logsService.SaveExcepcionDataBaseSybase(reqAutenticarse, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
                throw new Exception(reqAutenticarse.str_id_transaccion)!;
            }
            return respuesta;
        }


        public async Task<RespuestaTransaccion> SetIntentosFallidos ( ReqAutenticarse reqAutenticarse )
        {
            var respuesta = new RespuestaTransaccion( );

            try
            {
                DatosSolicitud ds = new DatosSolicitud( );

                Funciones.LlenarDatosAuditoria(ds, reqAutenticarse);
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_login", TipoDato = TipoDato.Integer, ObjValue = reqAutenticarse.str_id_usuario.ToString( ) });


                ds.NombreSP = "set_intentos_fallidos";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = objClienteDal.ExecuteDataSet(ds);
                var lst_valores = new List<ParametroSalidaValores>( );

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);
                var str_codigo = lst_valores.Find(x => x.StrNameParameter == "@int_o_error_cod")!.ObjValue;
                var str_error = lst_valores.Find(x => x.StrNameParameter == "@str_o_error")!.ObjValue.Trim( );

                respuesta.codigo = str_codigo.ToString( ).Trim( ).PadLeft(3, '0');
                respuesta.cuerpo = Funciones.ObtenerDatos(resultado);
                respuesta.diccionario.Add("str_error", str_error);

            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add("str_error", exception.ToString( ));
                await _logsService.SaveExcepcionDataBaseSybase(reqAutenticarse, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
            }
            return respuesta;
        }

    }
}
