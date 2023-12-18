using Application.Common.ISO20022.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.LoginUsuarioExterno.UsuarioExterno
{
    public class ReqLoginUsuarioExterno : Header, IRequest<ResLoginUsuarioExterno>
    {

        [Required]
        public string str_password { get; set; } = String.Empty;
        public string str_id_usr_ext { get; set; } = String.Empty;

    }
}
