using AccesoDatosGrpcAse.Neg;
using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.LogInMegomovil;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
  
    internal class KeysMegomovilDat : IKeysMegomovilDat
    {
        private readonly ApiSettings _settings;
        private readonly DALClient _objClienteDal;
        private readonly ILogs _logsService;
        private readonly string str_clase;

        public KeysMegomovilDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService, DALClient objClienteDal)
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType().FullName!;
            _objClienteDal = objClienteDal;
        } 

        public RespuestaTransaccion getLLavesCifrado( Header reqValidarLogin, string str_identificador )
        {
            var respuesta = new RespuestaTransaccion();

            try
            {
                DatosSolicitud ds = new();

                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = str_identificador } );

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@error", TipoDato = TipoDato.VarChar });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@error_cod", TipoDato = TipoDato.Integer });

                ds.NombreSP = "get_llaves_cifrado";
                ds.NombreBD = _settings.DB_meg_appmovil;

                var resultado = _objClienteDal.ExecuteDataSet( ds );
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add( item );
                var str_codigo = lst_valores.Find( x => x.StrNameParameter == "@error_cod" )!.ObjValue;
                var str_error = lst_valores.Find( x => x.StrNameParameter == "@error" )!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft( 3, '0' );
                respuesta.cuerpo = Funciones.ObtenerDatos( resultado );
                respuesta.diccionario.Add( "str_error", str_error );

            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add( "str_error", exception.ToString() );
                _logsService.SaveExcepcionDataBaseSybase(reqValidarLogin, MethodBase.GetCurrentMethod()!.Name, exception, str_clase );
                throw new ArgumentException(reqValidarLogin.str_id_transaccion )!;
            }
            return respuesta;
        }
    }
}
