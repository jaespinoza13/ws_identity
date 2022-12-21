using Application.Common.ISO20022.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.LogInMegomovil.LogInCredenciales;

public class ReqValidarLogin : Header, IRequest<ResValidarLogin>
{

    [Required]
    public string str_password { get; set; } = String.Empty;
}
