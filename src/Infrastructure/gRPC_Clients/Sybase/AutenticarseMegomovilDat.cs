using AccesoDatosGrpcAse.Neg;
using Application.Common.Cryptography;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.LogInMegomovil.Megomovil;
using Grpc.Net.Client;
using Infrastructure.Common.Funciones;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using static AccesoDatosGrpcAse.Neg.DAL;

namespace Infrastructure.gRPC_Clients.Sybase
{
    internal class AutenticarseMegomovilDat : IAutenticarseMegomovilDat
    {
        private readonly ApiSettings _settings;
        private readonly ILogs _logsService;
        private readonly string str_clase;


        public AutenticarseMegomovilDat ( IOptionsMonitor<ApiSettings> options, ILogs logsService)
        {
            _settings = options.CurrentValue;
            _logsService = logsService;

            this.str_clase = GetType( ).FullName!;
        }


        public async Task<RespuestaTransaccion> getAutenticarCredenciales ( Header header )
        {
            var respuesta = new RespuestaTransaccion( );
            GrpcChannel grpcChannel = null!;
            DALClient _objClienteDal = null!;
            try
            {
                var ds = new DatosSolicitud( );

                (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = header.str_login });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_canal", TipoDato = TipoDato.VarChar, ObjValue = header.str_nemonico_canal });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_terminal", TipoDato = TipoDato.VarChar, ObjValue = header.str_ip_dispositivo });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_usuario", TipoDato = TipoDato.VarChar, ObjValue = "USR_BMO" });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_sistema", TipoDato = TipoDato.Integer, ObjValue = header.str_id_sistema });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_oficina", TipoDato = TipoDato.Integer, ObjValue = "1" });

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@int_usr_migrado", TipoDato = TipoDato.Integer });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@error_cod", TipoDato = TipoDato.Integer });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@error", TipoDato = TipoDato.VarChar });

                ds.NombreSP = "get_login_autenticar";
                ds.NombreBD = _settings.DB_meg_appmovil;

                var resultado = await _objClienteDal.ExecuteDataSetAsync(ds);

                var lst_valores = new List<ParametroSalidaValores>( );

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);

                respuesta.codigo = lst_valores.Find(x => x.StrNameParameter == "@error_cod")!.ObjValue.ToString( ).PadLeft(3, '0');
                respuesta.diccionario.Add("str_error", lst_valores.Find(x => x.StrNameParameter == "@error")!.ObjValue.Trim( ));

                if (respuesta.codigo == "000" || respuesta.codigo == "155")
                {
                    respuesta.cuerpo = Funciones.ObtenerDatos(resultado);
                    respuesta.diccionario.Add("int_usr_migrado", lst_valores.Find(x => x.StrNameParameter == "@int_usr_migrado")!.ObjValue.Trim( ));
                }
            }
            catch (Exception ex)
            {
                respuesta.codigo = "003";
                respuesta.diccionario.Add("str_error", "Error inesperado, intenta más tarde.");
                await _logsService.SaveExcepcionDataBaseSybase(header, header.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod()!.Name, str_clase, ex);
            }
            Funciones.setCloseConnection(grpcChannel);
            return respuesta;
        }

        /// <summary>
        /// Obtiene datos para autenticar la huella
        /// </summary>
        /// <param name="str_login"></param>
        /// <returns></returns>
        public async Task<RespuestaTransaccion> getAutenticarHuellaFaceID ( ReqValidarLogin header )
        {
            RespuestaTransaccion respuesta = new RespuestaTransaccion( );
            GrpcChannel grpcChannel = null!;
            DALClient _objClienteDal = null!;
            try
            {
                var ds = new DatosSolicitud( );

                (grpcChannel, _objClienteDal) = Funciones.getConnection(_settings.client_grpc_sybase!);

                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_login", TipoDato = TipoDato.VarChar, ObjValue = header.str_login });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_canal", TipoDato = TipoDato.VarChar, ObjValue = header.str_nemonico_canal });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_identificador", TipoDato = TipoDato.VarChar, ObjValue = header.str_identificador });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_terminal", TipoDato = TipoDato.VarChar, ObjValue = header.str_ip_dispositivo });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@str_usuario", TipoDato = TipoDato.VarChar, ObjValue = "USR_BMO" });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_sistema", TipoDato = TipoDato.Integer, ObjValue = header.str_id_sistema });
                ds.ListaPEntrada.Add(new ParametroEntrada { StrNameParameter = "@int_oficina", TipoDato = TipoDato.Integer, ObjValue = "1" });

                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@error_cod", TipoDato = TipoDato.Integer });
                ds.ListaPSalida.Add(new ParametroSalida { StrNameParameter = "@error", TipoDato = TipoDato.VarChar });

                ds.NombreSP = "get_huella_autenticar";
                ds.NombreBD = _settings.DB_meg_appmovil;

                var resultado = await _objClienteDal.ExecuteDataSetAsync(ds);

                var lst_valores = new List<ParametroSalidaValores>( );

                foreach (var item in resultado.ListaPSalidaValores) lst_valores.Add(item);

                respuesta.codigo = lst_valores.Find(x => x.StrNameParameter == "@error_cod")!.ObjValue.ToString( ).PadLeft(3, '0');
                respuesta.diccionario.Add("str_error", lst_valores.Find(x => x.StrNameParameter == "@error")!.ObjValue.Trim( ));

                if (respuesta.codigo == "000")
                {
                    respuesta.cuerpo = Funciones.ObtenerDatos(resultado);

                }
            }
            catch (Exception ex)
            {
                respuesta.codigo = "003";
                respuesta.diccionario.Add("str_error", "Error inesperado, intenta más tarde.");
                await _logsService.SaveExcepcionDataBaseSybase(header, header.str_id_servicio!.Replace("REQ_", ""), MethodBase.GetCurrentMethod()!.Name, str_clase, ex);
            }
            Funciones.setCloseConnection(grpcChannel);
            return respuesta;
        }

    }
}
