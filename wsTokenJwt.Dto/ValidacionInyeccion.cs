using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsTokenJwt.Dto
{
    public class ValidacionInyeccion
    {
        public string idHeader { get; set; } = String.Empty;
        public string str_servicio { get; set; } = String.Empty;
        public string str_valor { get; set; } = String.Empty;
        public string str_campo { get; set; } = String.Empty;
        public string str_mensaje { get; set; } = String.Empty;
        public string str_informacion_adicional { get; set; } = String.Empty;
        public string str_tipo { get; set; } = String.Empty;
        public string str_ip { get; set; } = String.Empty;
        public DateTime dtt_fecha { get; set; }
    }
}
