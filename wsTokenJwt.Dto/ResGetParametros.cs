using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsTokenJwt.Model;

namespace wsTokenJwt.Dto
{
    public class ResGetParametros : ResComun
    {
        public List<Parametro>? lst_parametros { get; set; }
    }
}
