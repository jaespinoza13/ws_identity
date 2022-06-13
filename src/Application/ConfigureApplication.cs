using MediatR;
using Application.Jwt;
using System.Reflection;
using FluentValidation;
using Application.Common.Behaviours;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureApplication
{
    public static IServiceCollection AddApplicationServices ( this IServiceCollection services )
    {
        //SERVICIOS
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly( ));
        services.AddMediatR(Assembly.GetExecutingAssembly( ));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddTransient<IGenerarToken, GenerarToken>( );

        return services;
    }
}