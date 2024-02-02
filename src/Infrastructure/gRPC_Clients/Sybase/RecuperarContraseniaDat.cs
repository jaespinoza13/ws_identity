
using AccesoDatosGrpcAse.Neg;
using Application.Acceso.RecuperarContrasenia;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Grpc.Net.Client;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase;

public class RecuperarContraseniaDat : IAccesoDat
{
    private readonly ApiSettings _settings;
    private readonly ILogs _logsService;
    private readonly string str_clase;
    private const string str_mensaje_error = "Error inesperado, intenta más tarde.";

    public RecuperarContraseniaDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService)
    {
        _settings = options.CurrentValue;
        _logsService = logsService;

        this.str_clase = GetType( ).FullName!;
    }

    public async Task<RespuestaTransaccion> ValidaInfoRecuperacion ( ReqValidaInfoRecuparacion reqValidaInfo )
    {
        var respuesta = new RespuestaTransaccion( );
        GrpcChannel grpcChannel = null!;
        DALClient _objClienteDal = null!;
        try
        {
            DatosSolicitud ds = new( );

            (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

            Funciones.LlenarDatosAuditoria(ds, reqValidaInfo);

            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_num_documento", TipoDato = TipoDato.VarChar, ObjValue = reqValidaInfo.str_num_documento.ToString( ) });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_requerimiento", TipoDato = TipoDato.VarChar, ObjValue = reqValidaInfo.str_id_servicio.ToString( ) });

            ds.NombreSP = "validar_info_recuperacion";
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
            await _logsService.SaveExcepcionDataBaseSybase(reqValidaInfo, reqValidaInfo.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod()!.Name, GetType().FullName!, exception);
        }
        Funciones.setCloseConnection(grpcChannel);
        return respuesta;
    }

}
