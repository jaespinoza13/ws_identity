﻿
using System.Reflection;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Application.Common.Cryptography;
using static Application.Common.Cryptography.CryptographyRSA;

namespace Application.ParametrosSeguridad;
public class GetParametrosSeguridadHandler : IRequestHandler<ReqGetParametrosSeguridad, ResGetParametrosSeguridad>
{
    public readonly ILogs _logsService;
    private readonly string str_clase;
    public readonly IMemoryCache _memoryCache;
    private readonly IParametersInMemory _parameters;




    public GetParametrosSeguridadHandler ( ILogs logsService, IMemoryCache memoryCache, IParametersInMemory parameters )
    {
        this._logsService = logsService;
        this.str_clase = GetType().FullName!;
        this._memoryCache = memoryCache;
        _parameters = parameters;

    }

    public async Task<ResGetParametrosSeguridad> Handle(ReqGetParametrosSeguridad reqGetParametrosSeguridad, CancellationToken cancellationToken)
    {
        string str_operacion = "GET_PARAMETROS_SEGURIDAD";
        var respuesta = new ResGetParametrosSeguridad();
        respuesta.LlenarResHeader( reqGetParametrosSeguridad );
        await _logsService.SaveHeaderLogs(reqGetParametrosSeguridad, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);

        try
        {

            var date = _memoryCache.Get<DateTime>("Date_" + reqGetParametrosSeguridad.str_nemonico_canal);
            if (DateTime.Compare(DateTime.Now, date.AddHours(24)) > 0)
            {
                var KeyCreate = CryptographyRSA.GenerarLlavePublicaPrivada();
                _memoryCache.Set<DatosLlaveRsa>("Key_"+reqGetParametrosSeguridad.str_nemonico_canal, KeyCreate);
                _memoryCache.Set<DateTime>("Date_" + reqGetParametrosSeguridad.str_nemonico_canal, DateTime.Now);
            }
            var Key = _memoryCache.Get<DatosLlaveRsa>("Key_" + reqGetParametrosSeguridad.str_nemonico_canal);
            respuesta.datos_parametros.str_mod = Key.str_modulo;
            respuesta.datos_parametros.str_exp = Key.str_exponente;
            respuesta.datos_parametros.int_t_inac = Convert.ToInt32(_parameters.FindParametro("POL_TMP_MAX_INACTIVIDAD_" + reqGetParametrosSeguridad.str_nemonico_canal.ToUpper( )).str_valor_ini);
            respuesta.datos_parametros.int_t_otp = Convert.ToInt32(_parameters.FindParametro("TIEMPO_REG_OTP_" + reqGetParametrosSeguridad.str_nemonico_canal.ToUpper( )).str_valor_ini);

            respuesta.str_res_codigo = "000";



            await _logsService.SaveResponseLogs(respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
            return respuesta;
        }
        catch (Exception exception)
        {
            await _logsService.SaveExceptionLogs( respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase, exception );
            throw new ArgumentException( reqGetParametrosSeguridad.str_id_transaccion )!;
        }
       
    }

   
}