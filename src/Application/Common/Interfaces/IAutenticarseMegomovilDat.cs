
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.LogInMegomovil.Megomovil;

namespace Application.Common.Interfaces
{
    public interface IAutenticarseMegomovilDat
    {
        Task<RespuestaTransaccion> getAutenticarCredenciales ( Header header );
        Task<RespuestaTransaccion> getAutenticarHuellaFaceID ( ReqValidarLogin header );
    }
}
