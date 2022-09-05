
using Application.Common.ISO20022.Models;
using MediatR;

namespace Application.Common.Cryptography;

public class ReqGetKeys : Header, IRequest<ResGetKeys>
{

}
