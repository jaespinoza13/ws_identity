using Application.Common.ISO20022.Models;

namespace Application.ParametrosSeguridad
{
    public class ResGetParametrosSeguridad : ResComun
    {
        public DatosLlaveRespuesta datos_llave { get; set; } = new();

      
    }

   
 
    public class DatosLlaveRespuesta
    {
        public string? str_modulo { get; set; }
        public string? str_exponente { get; set; }

    }

}
