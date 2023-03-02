
using Application.Common.ISO20022.Models;

namespace Application.LogInMegomovil;

public class ResValidarLogin : ResComun
{
    public string? str_token { get; set; }
    public string str_password { get; set; } = "";
    public string str_token_dispositivo { get; set; } = "";
    public string str_identificador { get; set; } = "";
}