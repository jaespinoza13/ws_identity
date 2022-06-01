
using Application.Common.ISO20022.Models;
using Domain.Models;

namespace Application.LogIn;


public class ResAutenticarse : ResComun
{
    public Persona? objSocio { get; set; }
    public string? str_token { get; set; }
}