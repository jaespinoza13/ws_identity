
using MediatR;
using Application.Common.ISO20022.Models;

namespace Application.Acceso.RecuperarContrasenia;


public class ReqValidaInfo : Header, IRequest<ResValidaInfo>
{
    public string str_num_documento { get; set; } = String.Empty;
}

