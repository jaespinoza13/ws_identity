using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.ISO20022.Models;

namespace Application.LogInMegomovil.LoginInHuella
{
    public class ResValidarHuella:ResComun
    {
        public string str_token { get; set; } = "";
        public string str_password { get; set; } = "";
    }
}
