

using Application.Common.Cryptography;
using Application.Common.ISO20022.Models;

namespace Application.Acceso.RecuperarContrasenia;

public class ResValidaInfoRecuperacion : ResComun
{
    public DatosRecuperacion datos_recuperacion { get; set; } = new( );
    public string? str_token { get; set; }

    public void EncryptAES ( ResGetKeys Key )
    {
        if (Key == null)
        {
            str_login = "";
        }
        else
        {
            str_ente = CryptographyAES.Encrypt(str_ente!, Key.str_llave_simetrica);
            str_id_usuario = CryptographyAES.Encrypt(str_id_usuario!, Key.str_llave_simetrica);
            str_login = CryptographyAES.Encrypt(str_login!, Key.str_llave_simetrica);
            datos_recuperacion!.str_id_usuario = CryptographyAES.Encrypt(datos_recuperacion.str_id_usuario!, Key.str_llave_simetrica);
            datos_recuperacion.str_ente = CryptographyAES.Encrypt(datos_recuperacion.str_ente!, Key.str_llave_simetrica);
        }
    }
}

public class DatosRecuperacion
{
    public string str_ente { get; set; } = String.Empty;
    public string str_id_usuario { get; set; } = String.Empty;
    public int int_id_sol_rec { get; set; }
    public bool bl_requiere_otp { get; set; }
    public int int_validar_imagen { get; set; }
    public string str_mod { get; set; } = String.Empty;
    public string str_exp { get; set; } = String.Empty;

}