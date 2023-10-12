using WebUI.Filters;
using Application.LogIn;
using Microsoft.AspNetCore.Mvc;
using Application.Acceso.RecuperarContrasenia;
using Application.Common.ISO20022.Models;
using Application.LoginInvitado;
using Application.RecuperarReenvio;
using Application.LogInMegomovil.Megomovil;

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

        [ServiceFilter(typeof(CryptographyAESFilter))]
        [HttpPost("VALIDAR_INF_RECUP_REENVIO")]
        public Task<ResValidarInfRecupReenvio> ValidarInfRecupReactiva ( ReqValidarInfRecupReenvio ReqValidarInfRecupReenvio )
        {
            return Mediator.Send(ReqValidarInfRecupReenvio);
        }

        [HttpPost("VALIDAR_LOGIN_APP")]
        public IActionResult getValidarCredencialesApp ( object reqValidarLogin )
        {
            string str_identificador = HttpContext.Request.Headers["identificador"];
            string str_secreto = HttpContext.Request.Headers["secreto"];
            string str_id_transaccion = HttpContext.Request.Headers["id-transaccion"];
            string str_ip_publica = HttpContext.Request.Headers["real_ip"];
            var respuesta = Mediator.Send(new LogInMegomovilCommand(reqValidarLogin, str_identificador, str_secreto, str_id_transaccion, str_ip_publica)).Result;
            return Ok(respuesta);
        }
        [HttpPost("VALIDAR_HUELLA_APP")]
        public IActionResult getValidarHuellaApp ( object reqValidarLogin )
        {
            string str_identificador = HttpContext.Request.Headers["identificador"];
            string str_secreto = HttpContext.Request.Headers["secreto"];
            string str_id_transaccion = HttpContext.Request.Headers["id-transaccion"];
            string str_ip_publica = HttpContext.Request.Headers["real_ip"];
            var respuesta = Mediator.Send(new LoginInHuellaCommand(reqValidarLogin, str_identificador, str_secreto, str_id_transaccion, str_ip_publica)).Result;
            return Ok(respuesta);
        }
    }
}
