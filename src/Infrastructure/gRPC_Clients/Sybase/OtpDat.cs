using Microsoft.Extensions.Options;
using System.Reflection;

using AccesoDatosGrpcAse.Neg;
using static AccesoDatosGrpcAse.Neg.DAL;
using Grpc.Net.Client;

using Application.Common.Interfaces;
using Application.Common.Models;

using Infrastructure.Common.Funciones;
using Infrastructure.Common.Interfaces;
using Application.Common.ISO20022.Models;

namespace Infrastructure.gRPC_Clients.Sybase;

internal class OtpDat : IOtpDat
{
    private readonly ApiSettings _settings;
    private readonly ILogs _logsService;
    private readonly string str_clase;
    private const string str_mensaje_error = "Error inesperado, intenta más tarde.";

    public OtpDat(IOptionsMonitor<ApiSettings> options, ILogs logsService)
    {
        _settings = options.CurrentValue;
        _logsService = logsService;

        this.str_clase = GetType().FullName!;
    }


    public async Task<RespuestaTransaccion> GetDatosOtpDat(dynamic req_get_datos)
    {
        var respuesta = new RespuestaTransaccion();
        GrpcChannel grpcChannel = null!;
        DALClient _objClienteDal = null!;
        try
        {
            DatosSolicitud ds = new();

            (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@int_ente", TipoDato = TipoDato.Integer, ObjValue = req_get_datos.str_ente } );

            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@int_sistema", TipoDato = TipoDato.Integer, ObjValue = req_get_datos.str_id_sistema } );
            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_canal", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_nemonico_canal } );
            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_login } );
            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_equipo", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_ip_dispositivo } );
            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_sesion", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_sesion } );
            ds.ListaPEntrada.Add( new ParametroEntrada { StrNameParameter = "@str_mac", TipoDato = TipoDato.VarChar, ObjValue = req_get_datos.str_mac_dispositivo } );

            ds.ListaPSalida.Add( new ParametroSalida { StrNameParameter = "@o_error_cod", TipoDato = TipoDato.Integer } );
            ds.ListaPSalida.Add( new ParametroSalida { StrNameParameter = "@o_error", TipoDato = TipoDato.VarChar } );

            ds.NombreSP = "get_datos_otp_gen";
            ds.NombreBD = _settings.DB_meg_servicios;

            var resultado = await _objClienteDal.ExecuteDataSetAsync( ds );
            var lst_valores = new List<ParametroSalidaValores>();

            foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add( item );
            var str_codigo = lst_valores.Find( x => x.StrNameParameter == "@o_error_cod" )!.ObjValue;
            var str_error = lst_valores.Find( x => x.StrNameParameter == "@o_error" )!.ObjValue.Trim();

            respuesta.codigo = str_codigo.ToString().Trim().PadLeft( 3, '0' );
            respuesta.cuerpo = Funciones.ObtenerDatos( resultado );
            respuesta.diccionario.Add( "str_error", str_error.ToString() );

        }
        catch (Exception exception)
        {
            respuesta.codigo = "003";
            respuesta.diccionario.Add("str_error", str_mensaje_error);
            await _logsService.SaveExcepcionDataBaseSybase(req_get_datos, req_get_datos.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod()!.Name, GetType().FullName!, exception);
        }
        Funciones.setCloseConnection(grpcChannel);
        return respuesta;
    }
}