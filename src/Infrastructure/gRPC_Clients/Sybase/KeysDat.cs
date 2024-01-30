using AccesoDatosGrpcAse.Neg;
using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.LoginUsuarioExterno.UsuarioExterno;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
  
    internal class KeysDat : IKeysDat
    {
        private readonly ApiSettings _settings;
        private readonly DALClient _objClienteDal;
        private readonly ILogs _logsService;
        private readonly string str_clase;

        public KeysDat(IOptionsMonitor<ApiSettings> options, ILogs logsService, DALClient objClienteDal)
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType().FullName!;
            _objClienteDal = objClienteDal;
        }

        public RespuestaTransaccion AddKeys(ReqAddKeys reqAddKeys)
        {
            var respuesta = new RespuestaTransaccion();

            try
            {
                DatosSolicitud ds = new();

                Funciones.LlenarDatosAuditoria( ds, reqAddKeys );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@int_ente", TipoDato = TipoDato.Integer, ObjValue = reqAddKeys.str_ente } );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_modulo", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_modulo } );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_exponente", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_exponente } );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_llave_privada", TipoDato = TipoDato.Text, ObjValue = reqAddKeys.str_llave_privada } );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_llave_simetrica", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_llave_simetrica } );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_llave_secreta", TipoDato = TipoDato.VarChar, ObjValue = reqAddKeys.str_clave_secreta } );


                ds.NombreSP = "add_llaves_cifrado";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = _objClienteDal.ExecuteDataSet( ds );
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add( item );
                var str_codigo = lst_valores.Find( x => x.StrNameParameter == "@int_o_error_cod" )!.ObjValue;
                var str_error = lst_valores.Find( x => x.StrNameParameter == "@str_o_error" )!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft( 3, '0' );
                respuesta.cuerpo = Funciones.ObtenerDatos( resultado );
                respuesta.diccionario.Add( "str_error", str_error );

            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add( "str_error", exception.ToString() );
                _logsService.SaveExcepcionDataBaseSybase(reqAddKeys, reqAddKeys.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod( )!.Name, GetType( ).FullName!, exception);
                throw new ArgumentException( reqAddKeys.str_id_transaccion )!;
            }
            return respuesta;
        }

  

        public RespuestaTransaccion GetKeys(ReqGetKeys reqGetKeys)
        {
            var respuesta = new RespuestaTransaccion();

            try
            {
                DatosSolicitud ds = new();

                Funciones.LlenarDatosAuditoria( ds, reqGetKeys );
                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_llave_secreta", TipoDato = TipoDato.VarChar, ObjValue = reqGetKeys.str_clave_secreta } );


                ds.NombreSP = "get_llaves_cifrado";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = _objClienteDal.ExecuteDataSet( ds );
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add( item );
                var str_codigo = lst_valores.Find( x => x.StrNameParameter == "@int_o_error_cod" )!.ObjValue;
                var str_error = lst_valores.Find( x => x.StrNameParameter == "@str_o_error" )!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft( 3, '0' );
                respuesta.cuerpo = Funciones.ObtenerDatos( resultado );
                respuesta.diccionario.Add( "str_error", str_error );

            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add( "str_error", exception.ToString() );
                _logsService.SaveExcepcionDataBaseSybase(reqGetKeys, reqGetKeys.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod( )!.Name, GetType( ).FullName!, exception);

                throw new ArgumentException( reqGetKeys.str_id_transaccion )!;
            }
            return respuesta;
        }
    }
}
