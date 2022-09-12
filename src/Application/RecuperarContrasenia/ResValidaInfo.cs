

using Application.Common.ISO20022.Models;

namespace Application.Acceso.RecuperarContrasenia;

public class ResValidaInfo : ResComun
{
    public dynamic datos_recuperacion { get; set; } = new System.Dynamic.ExpandoObject( );
    public string? str_token { get; set; }
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