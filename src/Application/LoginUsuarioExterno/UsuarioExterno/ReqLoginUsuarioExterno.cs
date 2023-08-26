using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using static Application.Common.Cryptography.CryptographyRSA;

namespace Application.LoginUsuarioExterno.UsuarioExterno
{
    public class ReqLoginUsuarioExterno : Header, IRequest<ResLoginUsuarioExterno>
    {
        // Aqui no se puede desencriptar ya que no sabemos 
        // si los usuarios externos enviaran encriptaos estos datos

        [Required]
        //public string str_login { get; set; } = String.Empty;
        public string str_password { get; set; } = String.Empty;
        public string str_id_usr_ext { get; set; }

    }
}
