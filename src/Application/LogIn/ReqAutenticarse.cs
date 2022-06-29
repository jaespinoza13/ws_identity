
using Application.Common.ISO20022.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.LogIn;

public class ReqAutenticarse : Header, IRequest<ResAutenticarse>
{

    [Required]
    public string str_password { get; set; } = String.Empty; 
}
