using Application.Common.ISO20022.Interfaces;
using Application.Common.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.ISO20022.Models
{
    public class Header : IHeader
    {
        /// <summary>
        /// Id de log
        /// </summary>
        public string str_id_transaccion { get; set; } = String.Empty;
        /// <summary>
        /// Enete del socio
        /// </summary>
        public string? str_ente { get; set; }
        /// <summary>
        /// Nemonico del canal Ejm: CANBEEBOT
        /// </summary>
        /// 
        [Required]
        //[ValidaCanalAttribute]
        public string str_nemonico_canal { get; set; } = String.Empty;
        /// <summary>
        /// Id del sistema Ejm: 74
        /// </summary>
        /// 
        [Required]
        public string str_id_sistema { get; set; } = String.Empty;
        /// <summary>
        /// Nombre de la app Ejm: MEGONLINE
        /// </summary>
        /// 
        [Required]
        public string str_app { get; set; } = String.Empty;
        /// <summary>
        /// Id del servicio web Ejm: REQ_VALIDAR_USUARIO
        /// </summary>
        /// 
        [Required]
        public string str_id_servicio { get; set; } = String.Empty;
        /// <summary>
        /// Versión del servicio web Ejm: 1.0
        /// </summary>
        /// 
        [Required]
        public string str_version_servicio { get; set; } = String.Empty;
        /// <summary>
        /// Id del usuario para ley protección de datos.
        /// </summary>
        /// 
        [Required]
        public string str_id_usuario { get; set; } = String.Empty;
        /// <summary>
        /// Dirección física
        /// </summary>
        /// 
        [Required]
        public string str_mac_dispositivo { get; set; } = String.Empty;
        /// <summary>
        /// Dirección Ip
        /// </summary>
        /// 
        [Required]
        public string str_ip_dispositivo { get; set; } = String.Empty;
        /// <summary>
        /// Remitente Ejm: RED_SOCIAL_FACEBOOK
        /// </summary>
        /// 
        [Required]
        public string str_remitente { get; set; } = String.Empty;
        /// <summary>
        /// Receptor Ejm: COOPMEGO
        /// </summary>
        /// 
        [Required]
        public string str_receptor { get; set; } = String.Empty;
        /// <summary>
        /// Tipo de petición REQ o RES
        /// </summary>
        /// 
        [Required]
        public string str_tipo_peticion { get; set; } = String.Empty;
        /// <summary>
        /// Id mensaje formato yyMMddHHmmssffff
        /// </summary>
        public string str_id_msj { get; set; } = String.Empty;
        /// <summary>
        /// Fecha formato yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime dt_fecha_operacion { get; set; } = DateTime.Now;
        /// <summary>
        /// Posible duplicado false o true
        /// </summary>
        public bool bl_posible_duplicado { get; set; } = false;
        /// <summary>
        /// Prioridad
        /// </summary>
        public string str_prioridad { get; set; } = String.Empty;
        /// <summary>
        /// Login de usuario
        /// </summary>
        /// 
        [Required]
        public string str_login { get; set; } = String.Empty;
        /// <summary>
        /// Latitud
        /// </summary>
        /// 
        [Required]
        public string str_latitud { get; set; } = String.Empty;
        /// <summary>
        /// Longitud
        /// </summary>
        /// 
        [Required]
        public string str_longitud { get; set; } = String.Empty;
        /// <summary>
        /// Firma digital
        /// </summary>
        public string str_firma_digital { get; set; } = String.Empty;
        /// <summary>
        /// Num sim
        /// </summary>
        public string str_num_sim { get; set; } = String.Empty;
        /// <summary>
        /// Clave secreta
        /// </summary>
        public string str_clave_secreta { get; set; } = String.Empty;
        /// <summary>
        /// País
        /// </summary>
        /// 
        [Required]
        public string str_pais { get; set; } = String.Empty;
        /// <summary>
        /// Sesión
        /// </summary>
        /// 
        [Required]
        public string str_sesion { get; set; } = String.Empty;
        /// <summary>
        /// Id de Oficina
        /// </summary>
        /// 
        public string str_id_oficina { get; set; } = String.Empty;
        /// <summary>
        /// Id de Perfil
        /// </summary>
        /// 
        public string str_id_perfil { get; set; } = String.Empty;

        public void LlenarResHeader(Header requerimiento)
        {
            str_nemonico_canal = requerimiento.str_nemonico_canal;
            str_app = requerimiento.str_app;
            str_id_servicio = requerimiento.str_id_servicio != null ? requerimiento.str_id_servicio.Replace("REQ", "RES") : String.Empty;
            str_version_servicio = requerimiento.str_version_servicio;
            str_mac_dispositivo = requerimiento.str_mac_dispositivo;
            str_ip_dispositivo = requerimiento.str_ip_dispositivo;
            str_remitente = requerimiento.str_receptor;
            str_receptor = requerimiento.str_remitente;
            str_tipo_peticion = "RES";
            str_id_msj = DateTime.Now.ToString("yyMMddHHmmssffff");
            dt_fecha_operacion = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
            bl_posible_duplicado = requerimiento.bl_posible_duplicado;
            str_prioridad = requerimiento.str_prioridad;
            str_login = requerimiento.str_login;
            str_latitud = requerimiento.str_latitud;
            str_longitud = requerimiento.str_longitud;
            str_firma_digital = requerimiento.str_firma_digital;
            str_num_sim = requerimiento.str_num_sim;
            str_clave_secreta = requerimiento.str_clave_secreta;
        }
    }
}
