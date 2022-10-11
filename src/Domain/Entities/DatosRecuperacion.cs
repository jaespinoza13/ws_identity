using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DatosRecuperacion
    {
        public string str_ente { get; set; } = String.Empty;
        public string str_id_usuario { get; set; } = String.Empty;
        public int int_id_sol_rec { get; set; }
        public bool bl_requiere_otp { get; set; }
        public int int_validar_imagen { get; set; }
        public string str_mod { get; set; } = String.Empty;
        public string str_exp { get; set; } = String.Empty;
        public string str_token { get; set; } = String.Empty;

    }
}
