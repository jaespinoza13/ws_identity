namespace Application.Common.ISO20022.Interfaces;
public interface IException
{
    DateTime dt_res_fecha_msj_crea { get; set; }
    string str_res_estado_transaccion { get; set; }
    string str_res_codigo { get; set; }
    string str_res_id_servidor { get; set; }
    string str_res_info_adicional { get; set; }

}