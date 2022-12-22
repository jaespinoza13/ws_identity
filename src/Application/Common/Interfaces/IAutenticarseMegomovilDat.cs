
using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Application.LogIn;
using Application.LogInMegomovil.LoginInHuella;

namespace Application.Common.Interfaces
{
    public interface IAutenticarseMegomovilDat
    {
        Task<RespuestaTransaccion> getAutenticarCredenciales ( Header header );
        Task<RespuestaTransaccion> getAutenticarHuellaFaceID ( ReqValidarHuella header );
    }
}
