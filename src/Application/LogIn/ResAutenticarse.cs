
using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;
using Domain.Entities;

namespace Application.LogIn;


public class ResAutenticarse : ResComun
{
    public Persona? objSocio { get; set; }
    public string? str_token { get; set; }

    public void EncryptAES ( ResGetKeys Key ) {
        if (Key == null)
        {
            str_login = "";
        }
        else {
            str_ente = CryptographyAES.Encrypt(str_ente!, Key.str_llave_simetrica);
            str_id_usuario = CryptographyAES.Encrypt(str_id_usuario!, Key.str_llave_simetrica);
            str_login = CryptographyAES.Encrypt(str_login!, Key.str_llave_simetrica);
            objSocio!.str_id_usuario=CryptographyAES.Encrypt(objSocio.str_id_usuario!, Key.str_llave_simetrica);
            objSocio.str_ente = CryptographyAES.Encrypt(objSocio.str_ente!, Key.str_llave_simetrica);
        }
    }
}