using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RecuperarReenvio
{
    public interface IValidarRecuperaciones
    {
       RespuestaTransaccion ValidarInfRecupReactiva ( ReqValidarInfRecupReenvio ReqValidarInfRecupReenvio );
    }
}
