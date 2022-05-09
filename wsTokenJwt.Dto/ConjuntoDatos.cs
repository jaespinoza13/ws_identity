﻿
namespace wsTokenJwt.Dto
{
    public class ConjuntoDatos
    {
        public List<Tabla> lst_tablas { get; set; }
    }

    public class Tabla
    {
        public List<Fila> lst_filas { get; set; }
    }

    public class Fila
    {
        public Dictionary<string, object> nombre_valor { get; set; }

        public Fila()
        {
            nombre_valor = new Dictionary<string, object>();
        }
    }

}
