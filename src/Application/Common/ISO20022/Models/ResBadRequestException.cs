
using Application.Common.ISO20022.Interfaces;

namespace Application.Common.ISO20022.Models;
public class ResBadRequestException : IException
{
    public Dictionary<string, string[]> obj_res_detalle_errores { get; set; } = new();
    public DateTime dt_res_fecha_msj_crea { get; set; }
    public string str_res_estado_transaccion { get; set; } = String.Empty;
    public string str_res_codigo { get; set; } = String.Empty;
    public string str_res_id_servidor { get; set; } = String.Empty;
    public string str_res_info_adicional { get; set; } = String.Empty;
}
