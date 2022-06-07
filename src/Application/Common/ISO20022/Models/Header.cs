using System.ComponentModel.DataAnnotations;

namespace Application.Common.ISO20022.Models
{
    public class Header
    {
        /// <summary>
        /// Id de log
        /// </summary>
        public string str_id_transaccion { get; set; } = String.Empty;
        /// <summary>
        /// Enete del socio
        /// </summary>
        [Required( ErrorMessage = "Ente es requerido" )]
        public string str_ente { get; set; } = String.Empty;
        /// <summary>
        /// Nemonico del canal Ejm: CANBEEBOT
        /// </summary>
        [Required( ErrorMessage = "Nemonico del canal es requerido" )]
        public string str_nemonico_canal { get; set; } = String.Empty;
        /// <summary>
        /// Id del sistema Ejm: 74
        /// </summary>
        /// 
        [Required( ErrorMessage = "Id de sistema es requerido" )]
        public string str_id_sistema { get; set; } = String.Empty;
        /// <summary>
        /// Nombre de la app Ejm: MEGONLINE
        /// </summary>
        /// 
        [Required( ErrorMessage = "Nombre de App es requerido" )]
        public string str_app { get; set; } = String.Empty;
        /// <summary>
        /// Id del servicio web Ejm: REQ_VALIDAR_USUARIO
        /// </summary>
        /// 
        [Required( ErrorMessage = "Id del servicio es requerido" )]
        public string str_id_servicio { get; set; } = String.Empty;
        /// <summary>
        /// Versión del servicio web Ejm: 1.0
        /// </summary>
        /// 
        [Required( ErrorMessage = "Se requeriere versionamiento" )]
        public string str_version_servicio { get; set; } = String.Empty;
        /// <summary>
        /// Id del usuario para ley protección de datos.
        /// </summary>
        /// 
        [Required( ErrorMessage = "El id del usuario es requerido " )]
        public string str_id_usuario { get; set; } = String.Empty;
        /// <summary>
        /// Dirección física
        /// </summary>
        /// 
        [Required( ErrorMessage = "Parametro requerido" )]
        public string str_mac_dispositivo { get; set; } = String.Empty;
        /// <summary>
        /// Dirección Ip
        /// </summary>
        /// 
        [Required( ErrorMessage = "Ip requerida" )]
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
        [Required( ErrorMessage = "Especificar el tipo de petición" )]
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
        [Required( ErrorMessage = "Login es requerido" )]
        public string str_login { get; set; } = String.Empty;
        /// <summary>
        /// Latitud
        /// </summary>
        /// 
        [Required( ErrorMessage = "Parametro requerido" )]
        public string str_latitud { get; set; } = String.Empty;
        /// <summary>
        /// Longitud
        /// </summary>
        /// 
        [Required( ErrorMessage = "Parametro requerido" )]
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
        [Required( ErrorMessage = "Especificar el pais" )]
        public string str_pais { get; set; } = String.Empty;
        /// <summary>
        /// Sesión
        /// </summary>
        /// 
        [Required( ErrorMessage = "Sesion requerida" )]
        public string str_sesion { get; set; } = String.Empty;
        /// <summary>
        /// Id de Oficina
        /// </summary>
        /// 
        public string str_id_oficina { get; set; } = String.Empty;
        /// <summary>
        /// Id de Perfil
        /// </summary>
        public string str_id_perfil { get; set; } = String.Empty;
    }
}
