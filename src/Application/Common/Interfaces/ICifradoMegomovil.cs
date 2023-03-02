using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.ISO20022.Models;

namespace Application.Common.Interfaces
{
    public interface ICifradoMegomovil
    {
        void getLlavesCifrado ( Header header, string str_identificador, string str_clave_secreta );
        string encriptarTrama ( string str_campo );
        string desencriptarTrama ( string str_campo );
    }
}
