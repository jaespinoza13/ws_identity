
using Application.Common.Cryptography;
using Application.Common.Models;
using Application.LogIn;

namespace Application.Common.Interfaces
{
    public interface IAutenticarseDat
    {
        Task<RespuestaTransaccion> LoginDat ( ReqAutenticarse reqAutenticarse );
        Task<RespuestaTransaccion> SetIntentosFallidos ( ReqAutenticarse reqAutenticarse );
        Task<RespuestaTransaccion> AddKeys ( ReqAddKeys reqAddKeys );
    }
}
