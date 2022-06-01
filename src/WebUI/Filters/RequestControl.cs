using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebUI.Filters;

public class RequestControl : IActionFilter
{
    private readonly ILogger _logger;
    private readonly IParametersInMemory _parameters;
    private readonly IDailyRequest _dailyRequest;
    public RequestControl ( ILogger<RequestControl> logger, IParametersInMemory parameters, IDailyRequest daily )
    {
        this._logger = logger;
        this._parameters = parameters;
        this._dailyRequest = daily;
    }

    void IActionFilter.OnActionExecuting ( ActionExecutingContext context )
    {
        //VALIDACIÓN DE PARAMETROS
        _parameters.ValidaParametros ();

        //CONTROL DE PETICIONES DIARIAS
        var endpoint = context.HttpContext.Request.Path;
        string[] operacion = endpoint.Value!.Split ("/");
        _dailyRequest.controlPeticionesDiaras (operacion[3].ToUpper ());
    }

    void IActionFilter.OnActionExecuted ( ActionExecutedContext context )
    {
        _logger.LogInformation ("Despues de la peticion");
    }

}

