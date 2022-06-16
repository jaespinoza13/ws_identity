using Application.Acceso.RecuperarContrasenia;
using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IAccesoDat
{
    Task<RespuestaTransaccion> ValidaInfoRecuperacion(ReqValidaInfo reqValidaInfo);
}