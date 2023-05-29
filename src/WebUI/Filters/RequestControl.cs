using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

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
        if (context.ModelState.IsValid)
        {
            //VALIDACIÓN DE PARAMETROS
            _parameters.ValidaParametros( );

            //CONTROL DE PETICIONES DIARIAS
            var endpoint = context.HttpContext.Request.Path;
            string[] operacion = endpoint.Value!.Split("/");
            Task.Run(( ) => _dailyRequest.controlPeticionesDiaras(operacion[3].ToUpper( )));
        }
        else
        {
            throw new ValidationException( );
        }
    }

    void IActionFilter.OnActionExecuted ( ActionExecutedContext context )
    {
        _logger.LogInformation("");
    }

}

