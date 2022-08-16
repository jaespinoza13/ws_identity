using Application.Common.ISO20022.Models;

namespace Application.ParametrosSeguridad
{
    public class ResGetParametrosSeguridad : ResComun
    {
        public DatosLlaveRespuesta datos_llave { get; set; } = new();

      
    }

   
    public class DatosLlaveRsa
    {
        public string? str_modulo { get; set; }
        public string? str_exponente { get; set; }
        public string? str_xml_pub { get; set; }
        public string? str_xml_priv { get; set; }
    }
    public class DatosLlaveRespuesta
    {
        public string? str_modulo { get; set; }
        public string? str_exponente { get; set; }

    }

}
