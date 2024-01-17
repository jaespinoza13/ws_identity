using AccesoDatosGrpcAse.Neg;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Grpc.Net.Client;
using Infrastructure.Common.Funciones;
using Infrastructure.MemoryCache;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase;
internal class ParametrosDat : IParametrosDat
{
    private readonly ApiSettings _settings;
    private readonly DALClient _objClienteDal;
    private readonly ILogs _logsService;
    private readonly string str_clase;

    public ParametrosDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService, DALClient objClienteDal )
    {
        _settings = options.CurrentValue;
        _logsService = logsService;

        this.str_clase = GetType( ).FullName!;

        _objClienteDal = objClienteDal;
    }

    public RespuestaTransaccion GetParametrosDat ( dynamic req_get_parametros )
    {
        var respuesta = new RespuestaTransaccion( );

        try
        {
            DatosSolicitud ds = new( );
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_id_transaccion", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_id_transaccion });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_sistema", TipoDato = TipoDato.Integer, ObjValue = req_get_parametros.str_id_sistema });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_login });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nemonico_canal", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_nemonico_canal });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_ip_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_ip_dispositivo });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_sesion", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_sesion });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_mac_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_mac_dispositivo });

            ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
            ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });

            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nombre", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_nombre.ToString( ) });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nemonico", TipoDato = TipoDato.VarChar, ObjValue = req_get_parametros.str_nemonico.ToString( ) });
            ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_front", TipoDato = TipoDato.Integer, ObjValue = 0.ToString( ) });

            ds.NombreSP = "get_parametros";
            ds.NombreBD = _settings.DB_meg_megonline;

            var resultado = _objClienteDal.ExecuteDataSet(ds);
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
            _logsService.SaveExcepcionDataBaseSybase(req_get_parametros, req_get_parametros.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod( )!.Name, GetType( ).FullName!, exception);
            throw new ArgumentException(req_get_parametros.str_id_transaccion)!;
        }
        return respuesta;

    }
}

