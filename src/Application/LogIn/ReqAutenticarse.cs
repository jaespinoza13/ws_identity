
using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using static Application.Common.Cryptography.CryptographyRSA;

namespace Application.LogIn;

public class ReqAutenticarse : Header, IRequest<ResAutenticarse>
{

    [Required]
    public string str_password { get; set; } = String.Empty;

    public void DecryptRSA ( DatosLlaveRsa Key )
    {
        str_login = Decrypt(str_login!, Key.str_xml_priv!);
        str_password = Decrypt(str_password!, Key.str_xml_priv!);

    }
}
