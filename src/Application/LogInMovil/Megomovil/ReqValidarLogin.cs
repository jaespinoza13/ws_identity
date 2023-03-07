using Application.Common.ISO20022.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.LogInMegomovil.Megomovil;

public class ReqValidarLogin : Header, IRequest<ResValidarLogin>
{

    [Required]
    public string str_password { get; set; } = String.Empty;
    public string str_identificador { get; set; } = "";
    public string str_token_dispositivo { get; set; } = "";

}
