using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;

namespace Application.LoginUsuarioExterno.UsuarioExterno
{
    public class ResLoginUsuarioExterno : ResComun
    {
        public string? str_token { get; set; }

        // Metodo que encripta el token generado
        public void EncryptAES ( ResGetKeys Key )
        {
            if (Key == null)
            {
                str_token = "";
            }
            else
            {
                str_token = CryptographyAES.Encrypt(str_token!, Key.str_llave_simetrica);
            }
        }
    }
}
