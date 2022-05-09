using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using wsTokenJwt.Model;
using wsTokenJwt.Neg;
using wsTokenJwt.Neg.Utils;

namespace wsTokenJwt.Controllers
{
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ApiController]
    public class wsTokenJwtController : ControllerBase
    {
        private readonly SettingsJwt settingsJwt;
        private readonly ServiceSettings serviceSettings;

        public wsTokenJwtController(IOptionsMonitor<SettingsJwt> optionsMonitor, IOptionsMonitor<ServiceSettings> optionsMonitorApi, IOptionsMonitor<LoadParameters> optionsMonitorParam)
        {
            serviceSettings = optionsMonitorApi.CurrentValue;
            settingsJwt = optionsMonitor.CurrentValue;

            if (DateTime.Compare(DateTime.Now, LoadConfigService.dt_fecha_codigos.AddDays(1)) > 0 || LoadConfigService.lst_errores.Count <= 0)
            {
                LoadConfigService.LoadConfiguration(serviceSettings, optionsMonitorParam.CurrentValue);
            }
        }

        // POST api/wsUtilidades
        [HttpPost]
        public IActionResult Transaccion(object raw, string str_operacion)
        {
            try
            {
                TokenJwtNeg obj_json_web_token = new(serviceSettings, settingsJwt);
                object respuesta = obj_json_web_token.procesarSolicitud(raw, str_operacion);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message)!;
            }
        }
    }
}
