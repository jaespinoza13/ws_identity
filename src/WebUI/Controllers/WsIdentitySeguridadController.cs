using WebUI.Filters;

// Authorization
using Microsoft.AspNetCore.Mvc;

//USE CASE
using Application.ParametrosSeguridad;
namespace WebUI.Controllers
{
    [Route("api/wsIdentity")]
    [ApiController]
    [ServiceFilter(typeof(RequestControl))]
    [ProducesResponseType( StatusCodes.Status200OK )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( StatusCodes.Status500InternalServerError )]
    public class WsIdentitySeguridadController : ApiControllerBase
    {
        [HttpPost( "GET_PARAMETROS_SEGURIDAD" )]
        public async Task<ResGetParametrosSeguridad> GetParametrosSeguridad(ReqGetParametrosSeguridad reqGetParametrosSeguridad)
        {
            return await Mediator.Send( reqGetParametrosSeguridad );
        }

    }
}
