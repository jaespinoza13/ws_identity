using wsTokenJwt.Model;

namespace wsTokenJwt.Dto
{
    public class ReqGetParametros : Header
    {
        public string str_nombre { get; set; } = "-1";
        public string str_nemonico { get; set; } = "-1";
        public int int_front { get; set; }
    }
}
