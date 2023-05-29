using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LlavesCifradoMovil
    {
        public string str_dispositivo { get; set; } = "";
        public string str_modulo { get; set; } = "";
        public string str_exponente { get; set; } = "";
        public string str_iv { get; set; } = "";
        public string str_llv_pub_priv { get; set; } = "";

    }
}
