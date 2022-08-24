
using Application.Common.Models;
using Application.LogIn;

namespace Application.Common.Interfaces
{
    public interface IKeysDat
    {
       RespuestaTransaccion GetKeys( ReqGetKeys reqGetKeys);
       RespuestaTransaccion AddKeys ( ReqAddKeys reqAddKeys );
    }
}
