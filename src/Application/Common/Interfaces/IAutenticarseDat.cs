
using Application.Common.Models;
using Application.LogIn;

namespace Application.Common.Interfaces
{
    public interface IAutenticarseDat
    {
        Task<RespuestaTransaccion> LoginDat ( ReqAutenticarse reqAutenticarse, string claveEncriptada );
        Task<RespuestaTransaccion> SetIntentosFallidos ( ReqAutenticarse reqAutenticarse );
    }
}
