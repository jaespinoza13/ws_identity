using WebUI.Filters;
using Application.LogIn;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Route("api/wsIdentity")]
    [ApiController]
    [ServiceFilter(typeof(RequestControl))]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class WsIdentityController : ApiControllerBase
    {

        [HttpPost("AUTENTICARSE")]
        public async Task<ResAutenticarse> LogIn ( ReqAutenticarse reqAutenticarse )
        {
            return await Mediator.Send(reqAutenticarse);
        }
    }
}
