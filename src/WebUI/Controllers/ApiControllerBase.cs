using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender> ();
}