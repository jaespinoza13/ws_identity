using Application.Common.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebUI.Filters;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices ( this IServiceCollection services, IConfiguration configuration )
    {
        // CUSTOMISE API EXCEPTIONS BEHAVIOUR
        services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>( ))
                .AddFluentValidation(x => x.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly( )));

        // CUSTOMISE DEFAULT API BEHAVIOUR
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        //CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins( ).WithMethods("POST").AllowAnyHeader( );
                    });
        });

        //SERVICES
        services.AddDataProtection( );
        services.AddMemoryCache( );
        services.AddOptions( );



        //FILTERS
        services.AddTransient<CryptographyRSAFilter>( );
        services.AddTransient<CryptographyAESFilter>( );
        services.AddTransient<RequestControl>( );

        //SWAGGER
        services.AddEndpointsApiExplorer( );
        services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo
           {
               Title = "Identity API",
               Version = "v1",
               Description = "The Identity service generates authentication tokens that allow access to the CoopMego services REST APIs."
           });
       });

        //CONFIGURATIONS

        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:DataBases"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:GrpcSettings"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:ConfigMongodb"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:LogsPath"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:LoadParameters"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:valida_peticiones_diarias"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:HttpSettings"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:Endpoints"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:EndpointsAuth"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:ControlExcepciones"));
        services.Configure<Roles>(configuration.GetSection("Roles"));

        return services;
    }
}