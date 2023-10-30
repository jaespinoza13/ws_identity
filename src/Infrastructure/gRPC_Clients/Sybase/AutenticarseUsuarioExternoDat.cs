using AccesoDatosGrpcAse.Neg;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.LoginUsuarioExterno.UsuarioExterno;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
    internal class AutenticarseUsuarioExternoDat : IAutenticarseUsuarioExternoDat
    {
        private readonly ApiSettings _settings;
        private readonly DALClient _objClienteDal;
        private readonly ILogs _logsService;
        private readonly string str_clase;

        public AutenticarseUsuarioExternoDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService, DALClient objClienteDal )
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType( ).FullName!;

            _objClienteDal = objClienteDal;
        }

        public async Task<RespuestaTransaccion> LoginUsuarioExternoDat ( ReqLoginUsuarioExterno reqLoginUsuariosExterno, string claveEncriptada )
        {
            var respuesta = new RespuestaTransaccion( );

            try
            { 
                DatosSolicitud ds = new( );
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_clave", TipoDato = TipoDato.VarChar, ObjValue = claveEncriptada });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = reqLoginUsuariosExterno.str_login });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_nemonico_canal", TipoDato = TipoDato.VarChar, ObjValue = reqLoginUsuariosExterno.str_nemonico_canal });

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });


                ds.NombreSP = "get_login_auth_usuario_externo";
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
                await _logsService.SaveExcepcionDataBaseSybase(reqLoginUsuariosExterno, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
                throw new ArgumentException(reqLoginUsuariosExterno.str_id_transaccion)!;
            }
            return respuesta;
        }

        public async Task<RespuestaTransaccion> SetIntentosFallidos ( ReqLoginUsuarioExterno reqLoginUsuarioExterno )
        {
            var respuesta = new RespuestaTransaccion( );
            
            try
            {
                DatosSolicitud ds = new DatosSolicitud( );

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_id_usr_ext", TipoDato = TipoDato.Integer, ObjValue = reqLoginUsuarioExterno.str_id_usr_ext.ToString( ) });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_oficina", TipoDato = TipoDato.Integer, ObjValue = reqLoginUsuarioExterno.str_id_oficina });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_id_sistema", TipoDato = TipoDato.Integer, ObjValue = reqLoginUsuarioExterno.str_id_sistema });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = reqLoginUsuarioExterno.str_login });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_ip_dispositivo", TipoDato = TipoDato.VarChar, ObjValue = reqLoginUsuarioExterno.str_ip_dispositivo });

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@str_o_error", TipoDato = TipoDato.VarChar });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_o_error_cod", TipoDato = TipoDato.Integer });


                ds.NombreSP = "set_intentos_fallidos_usr_ext";
                ds.NombreBD = _settings.DB_meg_servicios;

                var resultado = _objClienteDal.ExecuteDataSet(ds);
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
                respuesta.diccionario.Add("str_error", exception.ToString( ));
                await _logsService.SaveExcepcionDataBaseSybase(reqLoginUsuarioExterno, MethodBase.GetCurrentMethod( )!.Name, exception, str_clase);
                throw new ArgumentException(reqLoginUsuarioExterno.str_id_transaccion)!;
            }
            return respuesta;
        }
    }
}