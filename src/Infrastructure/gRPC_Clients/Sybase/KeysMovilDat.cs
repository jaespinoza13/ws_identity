using AccesoDatosGrpcAse.Neg;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Grpc.Net.Client;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
  
    internal class KeysMovilDat : IKeysMovilDat
    {
        private readonly ApiSettings _settings;
        private readonly ILogs _logsService;
        private readonly string str_clase;
        private const string str_mensaje_error = "Error inesperado, intenta más tarde.";

        public KeysMovilDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService)
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType().FullName!;
        } 

        public RespuestaTransaccion getLLavesCifradoMovil( Header header, string str_identificador )
        {
            var respuesta = new RespuestaTransaccion();
            GrpcChannel grpcChannel = null!;
            DALClient _objClienteDal = null!;
            try
            {
                DatosSolicitud ds = new();

                (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

                ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = str_identificador } );

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });

                ds.NombreSP = "get_llaves_descifrado";
                ds.NombreBD = getBaseCifrado(header);

                var resultado = _objClienteDal.ExecuteDataSet( ds );
                var lst_valores = new List<ParametroSalidaValores>();

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add( item );
                var str_codigo = lst_valores.Find( x => x.StrNameParameter == "@int_o_error_cod")!.ObjValue;
                var str_error = lst_valores.Find( x => x.StrNameParameter == "@str_o_error" )!.ObjValue.Trim();

                respuesta.codigo = str_codigo.ToString().Trim().PadLeft( 3, '0' );
                respuesta.cuerpo = Funciones.ObtenerDatos( resultado );
                respuesta.diccionario.Add( "str_error", str_error );

            }
            catch (Exception exception)
            {
                respuesta.codigo = "001";
                respuesta.diccionario.Add("str_error", str_mensaje_error);
                _logsService.SaveExcepcionDataBaseSybase(header, header.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod()!.Name, GetType().FullName!, exception);
            }
            Funciones.setCloseConnection(grpcChannel);
            return respuesta;
        }
        private string getBaseCifrado ( Header header )
        {
            string str_base = "";
            switch (header.str_nemonico_canal)
            {
                case "CANBMO":
                    str_base = _settings.DB_meg_appmovil!;
                    break;
            }
            return str_base;
        }

    }
}
