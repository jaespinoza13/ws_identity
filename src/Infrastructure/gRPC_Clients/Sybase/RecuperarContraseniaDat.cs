﻿
using AccesoDatosGrpcAse.Neg;
using Application.Acceso.RecuperarContrasenia;
using Application.Common.Interfaces;
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
    private readonly DALClient _objClienteDal;
    private readonly ILogs _logsService;
    private readonly string str_clase;

    public RecuperarContraseniaDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService, DALClient objClienteDal )
    {
        _settings = options.CurrentValue;
        _logsService = logsService;

        this.str_clase = GetType( ).FullName!;

        _objClienteDal = objClienteDal;
    }

    public async Task<RespuestaTransaccion> ValidaInfoRecuperacion ( ReqValidaInfoRecuparacion reqValidaInfo )
    {
        var respuesta = new RespuestaTransaccion( );

        try
        {
            DatosSolicitud ds = new( );
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
            respuesta.diccionario.Add("str_error", exception.ToString( ));
            _logsService.SaveExcepcionDataBaseSybase(reqValidaInfo, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
            throw new ArgumentException(reqValidaInfo.str_id_transaccion)!;

        }
        return respuesta;
    }

}
