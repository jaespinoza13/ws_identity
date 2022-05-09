using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsTokenJwt.Model
{
    public interface IHeader
    {
        string str_id_transaccion { get; set; }
        string str_nemonico_canal { get; set; }
        string str_app { get; set; }
        string str_id_servicio { get; set; }
        string str_version_servicio { get; set; }
        string str_mac_dispositivo { get; set; }
        string str_ip_dispositivo { get; set; }
        string str_remitente { get; set; }
        string str_receptor { get; set; }
        string str_tipo_peticion { get; set; }
        string str_id_msj { get; set; }
        DateTime dt_fecha_operacion { get; set; }
        bool bl_posible_duplicado { get; set; }
        string str_token { get; set; }
        string str_prioridad { get; set; }
        string str_login { get; set; }
        string str_latitud { get; set; }
        string str_longitud { get; set; }
        string str_firma_digital { get; set; }
        string str_num_sim { get; set; }
        string str_clave_secreta { get; set; }
        string str_pais { get; set; }
        string str_sesion { get; set; }
        string str_id_oficina { get; set; }
        string str_id_perfil { get; set; }

        void LlenarResHeader(Header requerimiento);
    }
}
