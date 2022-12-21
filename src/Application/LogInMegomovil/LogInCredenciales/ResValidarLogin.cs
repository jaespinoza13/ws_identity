
using Application.Common.ISO20022.Models;

namespace Application.LogInMegomovil.LogInCredenciales;

public class ResValidarLogin : ResComun
{
    public string? str_token { get; set; }
    public string str_password { get; set; } = "";
}