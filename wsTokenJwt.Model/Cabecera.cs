
namespace wsTokenJwt.Model
{
    public class Cabecera
    {
        public int int_sistema { get; set; }
        public string str_usuario { get; set; } = String.Empty;
        public int int_perfil { get; set; }
        public int int_oficina { get; set; }
        public int int_canal { get; set; }
        public string str_nemonico_canal { get; set; } = String.Empty;
        public string str_ip { get; set; } = String.Empty;
        public string str_session { get; set; } = String.Empty;
        public string str_mac { get; set; } = String.Empty;
        public string str_nombre_canal { get; set; } = String.Empty;
    }
}
