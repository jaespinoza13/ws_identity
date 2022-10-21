using Application.Common.ISO20022.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.RecuperarReenvio
{
    public class ReqValidarInfRecupReenvio : Header, IRequest<ResValidarInfRecupReenvio>
    {
        public string str_num_documento { get; set; } = String.Empty;
        public string str_correo { get; set; } = String.Empty;
    }
}
