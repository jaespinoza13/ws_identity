using Application.Common.Models;

namespace Infrastructure.MemoryCache
{
    public interface IParametrosDat
    {
        RespuestaTransaccion GetParametrosDat ( dynamic req_get_parametros );
    }
}
