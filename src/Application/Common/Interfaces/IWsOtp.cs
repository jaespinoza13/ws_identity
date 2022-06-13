using Application.Common.ISO20022.Models;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IWsOtp
    {
        Task<RespuestaTransaccion> ValidaRequiereOtp(Header header, string str_operacion);
        Task<RespuestaTransaccion> ValidaOtp(dynamic reqValidaOtp );
    }
}
