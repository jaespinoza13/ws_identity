using wsTokenJwt.Dat;
using wsTokenJwt.Dto;
using wsTokenJwt.Model;

namespace wsTokenJwt.Neg.Utils
{
    public static class LoadConfigService
    {

        public static List<Parametro> lst_errores { get; set; } = new();
        public static List<Parametro> lst_parametros { get; set; } = new();
        public static DateTime dt_fecha_codigos { get; set; }

        public static void LoadConfiguration(ServiceSettings settings, LoadParameters settingsParameters)
        {
            try
            {
                var lst_id_sistemas = settingsParameters.lst_codigos_error_sistemas;
                var lst_nombres_parametros = settingsParameters.lst_nombres_parametros;

                lst_parametros.Clear();
                ReqGetParametros req_get_parametros = new();

                foreach (var item in lst_id_sistemas)
                {
                    req_get_parametros.str_nombre = "CODIGOS_ERROR";
                    req_get_parametros.str_id_sistema = item.ToString();
                    RespuestaTransaccion res_tran = new UtilidadesParametrosDat(settings).get_parametros(req_get_parametros);
                    lst_errores.AddRange(Utils.ConvertConjuntoDatosToListClass<Parametro>((ConjuntoDatos)res_tran.cuerpo).Where(x => !lst_errores.Any(y => y.str_nemonico == x.str_nemonico)));

                }

                foreach (var item in lst_nombres_parametros)
                {
                    req_get_parametros.str_nombre = item;
                    req_get_parametros.str_id_sistema = "-1";
                    RespuestaTransaccion res_tran2 = new UtilidadesParametrosDat(settings).get_parametros(req_get_parametros);
                    lst_parametros.AddRange(Utils.ConvertConjuntoDatosToListClass<Parametro>((ConjuntoDatos)res_tran2.cuerpo).Where(x => !lst_parametros.Any(y => y.str_nemonico == x.str_nemonico)));
                }
                dt_fecha_codigos = DateTime.Now.Date;

            }
            catch (Exception)
            {
                throw new ArgumentException("Sin parametros");
            }
        }

        public static Parametro FindErrorCode(string str_codigo)
        {
            return lst_errores.Find(x => x.str_valor_ini == str_codigo)!;
        }
        public static Parametro FindParametro(string str_nemonico)
        {
            return lst_parametros.Find(x => x.str_nemonico == str_nemonico)!;
        }
    }
}
