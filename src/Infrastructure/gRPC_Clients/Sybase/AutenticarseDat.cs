using AccesoDatosGrpcAse.Neg;
using Application.Common.Cryptography;
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
        private readonly ILogs _logsService;
        private readonly string str_clase;
        private const string str_mensaje_error = "Error inesperado, intenta más tarde.";

        public AutenticarseDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService)
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType( ).FullName!;
        }


        public async Task<RespuestaTransaccion> LoginDat ( ReqAutenticarse reqAutenticarse, string claveEncriptada )
        {
            var respuesta = new RespuestaTransaccion( );
            GrpcChannel grpcChannel = null!;
            DALClient _objClienteDal = null!;
            try
            {
                DatosSolicitud ds = new( );

                (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

                Funciones.LlenarDatosAuditoria(ds, reqAutenticarse);
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_clave", TipoDato = TipoDato.VarChar, ObjValue = claveEncriptada });

                ds.NombreSP = "get_login_autenticar";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = await _objClienteDal.ExecuteDataSetAsync(ds);
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
                respuesta.diccionario.Add("str_error", str_mensaje_error);
                _ = _logsService.SaveExcepcionDataBaseSybase(reqAutenticarse, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
            }
            Funciones.setCloseConnection(grpcChannel);
            return respuesta;
        }


        public async Task<RespuestaTransaccion> SetIntentosFallidos ( ReqAutenticarse reqAutenticarse )
        {
            var respuesta = new RespuestaTransaccion( );
            GrpcChannel grpcChannel = null!;
            DALClient _objClienteDal = null!;
            try
            {
                DatosSolicitud ds = new DatosSolicitud( );
                (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);
                Funciones.LlenarDatosAuditoria(ds, reqAutenticarse);
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_login", TipoDato = TipoDato.Integer, ObjValue = reqAutenticarse.str_id_usuario.ToString( ) });


                ds.NombreSP = "set_intentos_fallidos";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = await _objClienteDal.ExecuteDataSetAsync(ds);
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
                respuesta.diccionario.Add("str_error", str_mensaje_error);
                _ = _logsService.SaveExcepcionDataBaseSybase(reqAutenticarse, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
            }
            Funciones.setCloseConnection(grpcChannel);
            return respuesta;
        }
        public async Task<RespuestaTransaccion> AddKeys ( ReqAddKeys reqAddKeys )
        {
            var respuesta = new RespuestaTransaccion( );
            GrpcChannel grpcChannel = null!;
            DALClient _objClienteDal = null!;
            try
            {
                DatosSolicitud ds = new( );

                (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

                Funciones.LlenarDatosAuditoria(ds, reqAddKeys);
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_ente", TipoDato = TipoDato.Integer, ObjValue = reqAddKeys.str_ente });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_modulo", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_modulo });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_exponente", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_exponente });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_llave_privada", TipoDato = TipoDato.Text, ObjValue = reqAddKeys.str_llave_privada });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_llave_simetrica", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_llave_simetrica });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_llave_secreta", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_clave_secreta });


                ds.NombreSP = "add_llaves_cifrado";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = await _objClienteDal.ExecuteDataSetAsync(ds);
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
                respuesta.diccionario.Add("str_error", str_mensaje_error);
                _ = _logsService.SaveExcepcionDataBaseSybase(reqAddKeys, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
            }
            Funciones.setCloseConnection(grpcChannel);
            return respuesta;
        }

    }
}
