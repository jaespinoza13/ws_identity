namespace Domain.Models
{
    public class Persona
    {
        public int int_id_usuario { get; set; }
        public string? str_documento { get; set; }
        public string? str_nombres { get; set; }
        public string? str_telefono { get; set; }
        public string? str_correo { get; set; }
        public int int_ente { get; set; }
        public int int_tipo_documento { get; set; }
        public string? str_ultimo_acceso { get; set; }
    }
}
