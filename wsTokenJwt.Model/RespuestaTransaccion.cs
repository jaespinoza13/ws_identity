namespace wsTokenJwt.Model
{
    public class RespuestaTransaccion
    {
        public object cuerpo { get; set; } = new();

        public string codigo { get; set; } = string.Empty;

        public Dictionary<string, string> diccionario { get; set; }

        public RespuestaTransaccion()
        {
            diccionario = new Dictionary<string, string>();
            cuerpo = new();
        }
    }
}
