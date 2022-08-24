
using Application.Common.ISO20022.Models;
namespace Application.LogIn;


public class ResGetKeys 
{
    public string str_modulo { get; set; } = String.Empty;
    public string str_exponente { get; set; } = String.Empty;
    public string str_llave_privada { get; set; } = String.Empty;
    public string str_llave_simetrica { get; set; } = String.Empty;
}