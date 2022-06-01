using MediatR;
using Application.Jwt;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureApplication
{
    public static IServiceCollection AddApplicationServices ( this IServiceCollection services )
    {

        services.AddTransient<IGenerarToken, GenerarToken> ();
        services.AddMediatR(Assembly.GetExecutingAssembly( ));

        return services;
    }
}