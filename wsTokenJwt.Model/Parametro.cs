﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsTokenJwt.Model
{
    public class Parametro
    {
        public string str_nombre { get; set; } = String.Empty;
        public string str_nemonico { get; set; } = String.Empty;
        public string str_descripcion { get; set; } = String.Empty;
        public string str_valor_ini { get; set; } = String.Empty;
        public string str_valor_fin { get; set; } = String.Empty;
        public string str_error { get; set; } = String.Empty;
    }
}
