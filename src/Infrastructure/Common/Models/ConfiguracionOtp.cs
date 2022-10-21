
namespace Infrastructure.Common.Models
{
    public class ConfiguracionOtp
    {
        public int int_ente_socio { get; set; }
        public string str_nombre_socio { get; set; } = String.Empty;
        public string str_correo { get; set; } = String.Empty;
        public string str_celular { get; set; } = String.Empty;

        public int int_tiempo { get; set; }
        public int int_intentos { get; set; }
        public int int_longitud { get; set; }
        public int int_tiempo_regenera { get; set; }



        public string str_canal { get; set; } = String.Empty;
        public string str_servicio { get; set; } = String.Empty;
        public string str_proceso { get; set; } = String.Empty;
    }
}
