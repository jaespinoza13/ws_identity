
using MediatR;
using Application.Common.ISO20022.Models;
using static Application.Common.Cryptography.CryptographyRSA;

namespace Application.Acceso.RecuperarContrasenia;


public class ReqValidaInfo : Header, IRequest<ResValidaInfo>
{
    public string str_num_documento { get; set; } = String.Empty;
    public void DecryptRSA ( DatosLlaveRsa Key )
    {
        str_login = Decrypt(str_login!, Key.str_xml_priv!);
    }
}

