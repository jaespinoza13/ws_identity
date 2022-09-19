using Application.Common.ISO20022.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RecuperarReenvio
{
    public class ResValidarInfRecupReenvio : ResComun
    {
        public dynamic datos_recuperacion { get; set; } = new System.Dynamic.ExpandoObject( );
        public class DatosRecuperacion
        {
            public int int_ente { get; set; }
            public int int_id_usuario { get; set; }
            public int int_id_sol_rec { get; set; }
            public bool bl_requiere_otp { get; set; }
            public int int_validar_imagen { get; set; }
            public string str_token { get; set; } = String.Empty;

        }

    }
}
