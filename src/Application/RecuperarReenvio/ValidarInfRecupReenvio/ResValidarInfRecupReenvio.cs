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
       

    }
}
