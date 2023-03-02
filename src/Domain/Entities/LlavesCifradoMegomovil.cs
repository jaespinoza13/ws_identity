using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LlavesCifradoMegomovil
    {
        public string lci_modulo { get; set; } = "";
        public string lci_exponente { get; set; } = "";
        public string lci_p { get; set; } = "";
        public string lci_q { get; set; } = "";
        public string lci_dp { get; set; } = "";
        public string lci_dq { get; set; } = "";
        public string lci_inversaq { get; set; } = "";
        public string lci_d { get; set; } = "";
        public string lci_iv { get; set; } = "";
    }
}
