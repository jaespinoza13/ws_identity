using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DatosAutenticarMegomovil
    {
        public string lgc_pk_id { get; set; } = "";
        public string lgc_ente { get; set; } = "";
        public string lgc_clave { get; set; } = "";
        public string lgc_clave_tmp { get; set; } = "";
        public string lgc_estado { get; set; } = "";
        public int int_usr_migrado { get; set; } = 0;
    }
}
