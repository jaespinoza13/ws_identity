using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using wsTokenJwt.Model;

namespace wsTokenJwt.Neg
{
    public class TokenJwtNeg
    {
        private readonly ServiceSettings serviceSettings;
        private readonly SettingsJwt _settingsJwt;
        public TokenJwtNeg(ServiceSettings settings, SettingsJwt settingsJwt)
        {
            this.serviceSettings = settings;
            this._settingsJwt = settingsJwt;
        }
        public object procesarSolicitud(Object sol_tran, string str_operacion)
        {
            object respuesta = new();
            try
            {
                switch (str_operacion)
                {
                    case "GENERAR_TOKEN":
                        var req_genera_otp = JsonSerializer.Deserialize<Header>(JsonSerializer.Serialize(sol_tran))!;
                        respuesta = new JsonWebTokenNeg(serviceSettings, _settingsJwt).GenerarToken(req_genera_otp, str_operacion);
                        break;

                    case "VALIDAR_TOKEN":
                        var req_valida_otp = JsonSerializer.Deserialize<Header>(JsonSerializer.Serialize(sol_tran))!;
                        respuesta = new JsonWebTokenNeg(serviceSettings, _settingsJwt).ValidarToken(req_valida_otp, str_operacion);
                        break;
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message)!;
            }
        }
    }
}
