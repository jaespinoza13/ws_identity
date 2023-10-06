using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Cryptography;
using Application.LoginUsuarioExterno.UsuarioExterno;
using Application.Common.Models;
namespace Application.Common.Interfaces
{
    public interface IAutenticarseUsuarioExternoDat
    {
        Task<RespuestaTransaccion> LoginUsuarioExternoDat ( ReqLoginUsuarioExterno reqLoginUsuarioExterno, string claveEncriptada );
        Task<RespuestaTransaccion> SetIntentosFallidos ( ReqLoginUsuarioExterno reqLoginUsuarioExterno );

    }
}
