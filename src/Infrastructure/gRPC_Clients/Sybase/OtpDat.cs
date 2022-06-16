using Microsoft.Extensions.Options;
using System.Reflection;

using AccesoDatosGrpcAse.Neg;
using static AccesoDatosGrpcAse.Neg.DAL;
using Grpc.Net.Client;

using Application.Common.Interfaces;
using Application.Common.Models;

using Infrastructure.Common.Funciones;
using Infrastructure.Common.Interfaces;

namespace Infrastructure.gRPC_Clients.Sybase;

internal class OtpDat : IOtpDat
{
    private readonly ApiSettings _settings;
    private readonly DALClient objClienteDal;
    private readonly ILogs _logsService;
    private readonly string str_clase;

    public OtpDat(IOptionsMonitor<ApiSettings> options, ILogs logsService)
    {
        _settings = options.CurrentValue;
        _logsService = logsService;

        this.str_clase = GetType().FullName!;

        var handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds( _settings.delayOutGrpcSybase ),
            KeepAlivePingTimeout = TimeSpan.FromSeconds( _settings.timeoutGrpcSybase ),
            EnableMultipleHttp2Connections = true
        };

        var canal = GrpcChannel.ForAddress( _settings.client_grpc_sybase!, new GrpcChannelOptions { HttpHandler = handler } );
        objClienteDal = new DALClient( canal );
    }


    public async Task<RespuestaTransaccion> GetDatosOtpDat(dynamic req_get_datos)
    {
        var respuesta = new RespuestaTransaccion();

        try
        {
            DatosSolicitud ds = new();
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

            var resultado = await objClienteDal.ExecuteDataSetAsync( ds );
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
            respuesta.diccionario.Add( "str_error", exception.ToString() );
            await _logsService.SaveExcepcionDataBaseSybase( req_get_datos, MethodBase.GetCurrentMethod()!.Name, exception, str_clase );
            throw new ArgumentException( req_get_datos.str_id_transaccion )!;
        }
        return respuesta;
    }
}