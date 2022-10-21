
using Application.Common.Cryptography;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IKeysDat
    {
       RespuestaTransaccion GetKeys( ReqGetKeys reqGetKeys);
       RespuestaTransaccion AddKeys ( ReqAddKeys reqAddKeys );
    }
}
