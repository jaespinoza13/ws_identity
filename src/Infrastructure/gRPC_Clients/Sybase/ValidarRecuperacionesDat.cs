using AccesoDatosGrpcAse.Neg;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.RecuperarReenvio;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
    public class ValidarRecuperacionesDat: IValidarRecuperaciones
    {
    private readonly ApiSettings _settings;
    private readonly DALClient _objClienteDal;
    private readonly ILogs _logsService;
    private readonly string str_clase;

    public ValidarRecuperacionesDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService, DALClient objClienteDal )
    {
        _settings = options.CurrentValue;
        _logsService = logsService;

        this.str_clase = GetType( ).FullName!;

        _objClienteDal = objClienteDal;
    }

        public RespuestaTransaccion ValidarInfRecupReactiva ( ReqValidarInfRecupReenvio ReqValidarInfRecupReenvio )
        {
            var respuesta = new RespuestaTransaccion( );

            try
            {
                DatosSolicitud ds = new( );
                Funciones.LlenarDatosAuditoria(ds, ReqValidarInfRecupReenvio);

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_num_documento", TipoDato = TipoDato.VarChar, ObjValue = ReqValidarInfRecupReenvio.str_num_documento.ToString( ) });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_correo", TipoDato = TipoDato.VarChar, ObjValue = ReqValidarInfRecupReenvio.str_correo.ToString( ) });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_requerimiento", TipoDato = TipoDato.VarChar, ObjValue = ReqValidarInfRecupReenvio.str_id_servicio.ToString( ) });

                ds.NombreSP = "validar_inf_recup_reactiva";
                ds.NombreBD = _settings.DB_meg_servicios;

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
                _logsService.SaveExcepcionDataBaseSybase(ReqValidarInfRecupReenvio, ReqValidarInfRecupReenvio.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod( )!.Name, GetType( ).FullName!, exception);
                throw new ArgumentException(ReqValidarInfRecupReenvio.str_id_transaccion)!;
            }
            return respuesta;
        }
    }
}
