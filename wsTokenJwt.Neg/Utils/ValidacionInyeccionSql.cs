
using wsTokenJwt.Model;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text.Json;
using wsTokenJwt.Dto;
using wsConsultas.Neg;

namespace wsTokenJwt.Neg.Utils
{
    public static class ValidacionInyeccionSql
    {

        public static Boolean validar_palabras_sql(string str_palabra_validar,string campo,string str_operacion, Header header,string str_clase)
        {
            ValidacionInyeccion validacion=new ();
            List<string> str_palabras_encontradas = new List<string> { };
            Boolean bln_encontro_palabra = false;
            Boolean bln_cadena_segura = false;
            string str_expresion_ini = @"(\W*|^)(";
            string str_expresion_fin = @")(\W*|$)";
            string str_expresion_cuerpo_palabras = "";

            string str_palabas_pattern = "";
            str_palabas_pattern += LoadConfigService.FindParametro("UTIL_SQL_INJECTION_1")!.str_valor_ini;
            str_palabas_pattern += LoadConfigService.FindParametro("UTIL_SQL_INJECTION_2")!.str_valor_ini;



            str_expresion_cuerpo_palabras = str_expresion_ini + str_palabas_pattern + str_expresion_fin;
            Match mtc_validar_expresion = Regex.Match(str_palabra_validar.ToLower(), str_expresion_cuerpo_palabras, RegexOptions.IgnoreCase);

            while (mtc_validar_expresion.Success)
            {
                bln_encontro_palabra = true;
                str_palabras_encontradas.Add(
                        mtc_validar_expresion.Value
                );
                mtc_validar_expresion = mtc_validar_expresion.NextMatch();
            }

            if (bln_encontro_palabra)
            {
                validacion.idHeader = header.str_id_transaccion;
                validacion.str_mensaje = "Solicitud con intento de inyección sql";
                validacion.str_informacion_adicional = "Listado de palabras encontradas " + JsonSerializer.Serialize(str_palabras_encontradas);
                validacion.str_servicio = str_operacion;
                validacion.str_campo = campo;
                validacion.str_ip = header.str_ip_dispositivo;
                validacion.str_valor = str_palabra_validar;
                validacion.str_tipo = "Inyección SQL";
                validacion.dtt_fecha = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);

                ServiceLogs.SaveAmenazasLogs(validacion, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase);
             

            }
            else
            {
                bln_cadena_segura = true;

            }
            return bln_cadena_segura;
        }



    }
}
