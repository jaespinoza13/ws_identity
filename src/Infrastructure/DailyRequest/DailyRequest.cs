using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.gGRPC_Clients.Mongo;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.DailyRequest;
public class PeticionDiaria
{
    public string _id { get; set; } = String.Empty;
    public int int_num_peticion { get; set; }
    public string str_fecha_solicitud { get; set; } = String.Empty;
    public string str_operacion { get; set; } = String.Empty;
}

public class DailyRequest : IDailyRequest
{
    public readonly ApiSettings _settings;
    public readonly IParametersInMemory _parameters;
    public readonly IMongoDat _mongo;
    public DailyRequest(IOptionsMonitor<ApiSettings> options, IParametersInMemory parameters, IMongoDat mongo)
    {
        this._settings = options.CurrentValue;
        this._parameters = parameters;
        this._mongo = mongo;
    }

    public bool controlPeticionesDiaras(string str_operacion)
    {
        bool respuesta = true;

        try
        {
            string str_fecha_diaria = DateTime.Now.ToString("yyyy-MM-dd");
            string str_filtro = "{'str_fecha_solicitud':'" + str_fecha_diaria + "','str_operacion':'" + str_operacion + "'}";

            RespuestaTransaccion var_respuesta = _mongo.buscar_peticiones_diarias(str_filtro);
            int int_act_peticiones = 1;
            if (var_respuesta.codigo == "000")
            {
                var resp_mongo = var_respuesta.cuerpo;
                if (resp_mongo != null && resp_mongo.ToString() != "[]")
                {
                    var res_datos_mongo = var_respuesta.cuerpo.ToString()!.Replace("ObjectId(", " ").Replace(")", " ");
                    res_datos_mongo = res_datos_mongo.Replace("[", "").Replace("]", "");
                    PeticionDiaria peticion_diaria = JsonSerializer.Deserialize<PeticionDiaria>(res_datos_mongo)!;
                    if (peticion_diaria!._id != null)
                    {
                        int_act_peticiones = peticion_diaria.int_num_peticion + 1;

                        int respuesta_promedio = _mongo.obtener_promedio(str_operacion);
                        var cantidad_maxima = respuesta_promedio * Convert.ToInt32(_parameters.FindParametro("PRM_MAXIMO_PETICIONES_DIARIAS")!.str_valor_ini) / 100;

                        if (_settings.valida_peticiones_diarias && int_act_peticiones > cantidad_maxima)
                            respuesta = false;
                    }
                }
                else
                {
                    _mongo.guardar_promedio_peticion_diaria(str_operacion, str_fecha_diaria);
                }

                string str_act_registro = "{$set:{'int_num_peticion':" + int_act_peticiones + ", 'str_fecha_solicitud' : '" + str_fecha_diaria + "',str_operacion:'" + str_operacion + "'}}";
                _mongo.actualizar_peticion_diaria(str_filtro, str_act_registro);
            }
        }
        catch (Exception)
        {
            respuesta = true;
        }
        return respuesta;
    }
}
