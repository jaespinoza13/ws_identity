
using Application.Common.ISO20022.Models;
using MediatR;

namespace Application.LogIn;

public class ReqGetKeys : Header, IRequest<ResGetKeys>
{

}
