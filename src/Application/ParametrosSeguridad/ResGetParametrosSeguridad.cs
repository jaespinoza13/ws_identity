using Application.Common.ISO20022.Models;

namespace Application.ParametrosSeguridad
{
    public class ResGetParametrosSeguridad : ResComun
    {
        public DatosLlaveRespuesta datos_parametros { get; set; } = new();

      
    }

   
 
    public class DatosLlaveRespuesta
    {
        public string? str_mod { get; set; }
        public string? str_exp { get; set; }

    }

}
