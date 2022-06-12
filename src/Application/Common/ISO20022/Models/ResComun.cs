namespace Application.Common.ISO20022.Models;
public class ResComun : Header
{
    public string str_res_original_id_msj { get; set; } = string.Empty;
    public string str_res_original_id_servicio { get; set; } = string.Empty;
    public DateTime dt_res_fecha_msj_crea { get; set; } = DateTime.Now;
    public string str_res_estado_transaccion { get; set; } = string.Empty;
    public string str_res_codigo { get; set; } = string.Empty;
    public string str_res_id_servidor { get; set; } = string.Empty;
    public string str_res_info_adicional { get; set; } = string.Empty;

    public void LlenarResHeader(Header requerimiento)
    {
        str_id_transaccion = Guid.NewGuid().ToString();
        str_nemonico_canal = requerimiento.str_nemonico_canal;
        str_app = requerimiento.str_app;
        str_id_servicio = requerimiento.str_id_servicio != null ? requerimiento.str_id_servicio.Replace( "REQ", "RES" ) : string.Empty;
        str_version_servicio = requerimiento.str_version_servicio;
        str_mac_dispositivo = requerimiento.str_mac_dispositivo;
        str_ip_dispositivo = requerimiento.str_ip_dispositivo;
        str_remitente = requerimiento.str_receptor;
        str_receptor = requerimiento.str_remitente;
        str_tipo_peticion = "REQ";
        str_id_msj = requerimiento.str_id_msj;
        dt_fecha_operacion = DateTime.ParseExact( DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ), "yyyy-MM-dd HH:mm:ss", null );
        bl_posible_duplicado = requerimiento.bl_posible_duplicado;
        str_prioridad = requerimiento.str_prioridad;
        str_login = requerimiento.str_login;
        str_latitud = requerimiento.str_latitud;
        str_longitud = requerimiento.str_longitud;
        str_firma_digital = requerimiento.str_firma_digital;
        str_num_sim = requerimiento.str_num_sim;
        str_clave_secreta = requerimiento.str_clave_secreta;
        str_pais = requerimiento.str_pais;
        str_sesion = requerimiento.str_sesion;
        str_id_perfil = requerimiento.str_id_perfil;
        str_id_oficina = requerimiento.str_id_oficina;
        str_ente = requerimiento.str_ente;
        requerimiento.str_id_transaccion = str_id_transaccion;
        str_res_original_id_msj = requerimiento.str_id_msj;
        str_res_original_id_servicio = requerimiento.str_id_servicio!;
        dt_res_fecha_msj_crea = requerimiento.dt_fecha_operacion;
    }
}