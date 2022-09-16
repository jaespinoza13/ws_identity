using WebUI.Filters;
using Application.LogIn;
using Microsoft.AspNetCore.Mvc;
using Application.Acceso.RecuperarContrasenia;
using Application.Common.ISO20022.Models;
using Application.LoginInvitado;

namespace WebUI.Controllers
{
    [Route("api/wsIdentity")]
    [ApiController]
    [ServiceFilter(typeof(RequestControl))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class WsIdentityController : ApiControllerBase
    {

        [HttpPost("autenticarse")]
        [ServiceFilter(typeof(CryptographyRSAFilter))]
        [ServiceFilter(typeof(CryptographyAESFilter))]
        public async Task<ResAutenticarse> LogIn ( ReqAutenticarse reqAutenticarse )

        {
            return await Mediator.Send(reqAutenticarse);
        }
        [ServiceFilter(typeof(CryptographyRSAFilter))]
        [ServiceFilter(typeof(CryptographyAESFilter))]
        [HttpPost("VALIDAR_INFO_RECUPERACION")]
        public async Task<ResValidaInfoRecuperacion> ValidaInfoRecuperacion ( ReqValidaInfoRecuparacion reqValidaInfo )
        {
            return await Mediator.Send(reqValidaInfo);
        }

        [HttpPost("AUTENTICARSE_INVITADO_INT")]
        public Task<ResAutenticarseInvitadoInt> AutenticarseInvitadoInterno ( Header header )
        {
            return Mediator.Send(new AutenticarseInvitadoIntCommand(header));
        }

        [HttpPost("AUTENTICARSE_INVITADO_EXT")]
        public Task<ResAutenticarseInvitadoExt> AutenticarseInvitadoExterno ( Header header )
        {
            return Mediator.Send(new AutenticarseInvitadoExtCommand(header));
        }
    }
}
