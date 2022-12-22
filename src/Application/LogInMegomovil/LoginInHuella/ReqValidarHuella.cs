using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.ISO20022.Models;
using MediatR;

namespace Application.LogInMegomovil.LoginInHuella
{
    public class ReqValidarHuella: Header, IRequest<ResValidarHuella>
    {
        public string str_identificador { get; set; } = "";
        public string str_password { get; set; } = "";
    }
}
