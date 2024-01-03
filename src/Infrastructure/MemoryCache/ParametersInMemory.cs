using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;

using Domain.Parameters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.MemoryCache;

public class GetParameters : Header
{
    public string str_nombre { get; set; } = "";
    public string str_nemonico { get; set; } = "-1";
    public int int_front { get; set; }
}

internal class ParametersInMemory : IParametersInMemory
{
    public readonly ApiSettings _settings;
    public readonly IParametrosDat _parametros;
    public readonly IMemoryCache _memoryCache;
    public DateTime dt_fecha_codigos { get; set; }

    public ParametersInMemory ( IOptionsMonitor<ApiSettings> options, IParametrosDat parametros, IMemoryCache memoryCache )
    {
        this._settings = options.CurrentValue;
        this._parametros = parametros;
        this._memoryCache = memoryCache;
    }

    public void LoadConfiguration ( )
    {
        try
        {
            var lst_id_sistemas = _settings.lst_codigos_error_sistemas;
            var lst_nombres_parametros = _settings.lst_nombres_parametros;

            List<Parametro> lst_errores = new();
            List<Parametro> lst_parametros = new();
            GetParameters req_get_parametros = new();

            lst_parametros.Clear();


            foreach (var item in lst_id_sistemas)
            {
                req_get_parametros.str_nombre = "CODIGOS_ERROR";
                req_get_parametros.str_id_sistema = item.ToString();
                RespuestaTransaccion res_tran = _parametros.GetParametrosDat(req_get_parametros);// VALIDAR
                lst_errores.AddRange(Conversions.ConvertConjuntoDatosToListClass<Parametro>((ConjuntoDatos)res_tran.cuerpo).Where(x => !lst_parametros.Any(y => y.str_nemonico == x.str_nemonico)));

            }

            foreach (var item in lst_nombres_parametros)
            {
                req_get_parametros.str_nombre = item;
                req_get_parametros.str_id_sistema = "-1";
                RespuestaTransaccion res_tran2 = _parametros.GetParametrosDat(req_get_parametros);// VALIDAR
                lst_parametros.AddRange(Conversions.ConvertConjuntoDatosToListClass<Parametro>((ConjuntoDatos)res_tran2.cuerpo).Where(x => !lst_parametros.Any(y => y.str_nemonico == x.str_nemonico)));
            }

            _memoryCache.Set<List<Parametro>>("Parametros", lst_parametros);
            _memoryCache.Set<List<Parametro>>("Errores", lst_errores);

            dt_fecha_codigos = DateTime.Now.Date;
        }
        catch (Exception)
        {
            throw new ArgumentException("Sin parametros");
        }
    }

    public void ValidaParametros ( )
    {
        if (DateTime.Compare(DateTime.Now, dt_fecha_codigos.AddDays(1)) > 0)
        {
            LoadConfiguration();
        }
    }

    public Parametro FindErrorCode ( string str_codigo )
    {
        var lst_errores = _memoryCache.Get<List<Parametro>>("Errores");
        return lst_errores.Find(x => x.str_valor_ini == str_codigo)!;
    }
    public Parametro FindParametro ( string str_nemonico )
    {
        var lst_parametros = _memoryCache.Get<List<Parametro>>("Parametros");
        return lst_parametros.Find(x => x.str_nemonico == str_nemonico)!;
    }
}
