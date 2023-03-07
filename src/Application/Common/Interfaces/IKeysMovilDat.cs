
using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.LogInMegomovil;

namespace Application.Common.Interfaces
{
    public interface IKeysMovilDat
    {
        RespuestaTransaccion getLLavesCifradoMegomovil ( Header header, string str_identificador);

    }
}
