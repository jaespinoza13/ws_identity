using Application.Common.Models;

namespace Infrastructure.Common.Interfaces
{
    public interface IOtpDat
    {
        Task<RespuestaTransaccion> get_datos_otp(dynamic req_get_datos);
    }
}
