﻿namespace Domain.Entities
{
    public class Persona
    {
        public string? str_id_usuario { get; set; }
        public string? str_documento { get; set; }
        public string? str_nombres { get; set; }
        public string? str_direccion { get; set; }
        public string? str_telefono { get; set; }
        public string? str_correo { get; set; }
        public string? str_ente { get; set; }
        public int int_tipo_documento { get; set; }
        public int int_id_perfil{ get; set; }
        public string? str_mod{ get; set; }
        public string? str_exp{ get; set; }
    }
}
