using Application.Common.Behaviours;
using Application.Common.Models;
using Microsoft.OpenApi.Models;
using WebUI.Filters;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices ( this IServiceCollection services, IConfiguration configuration )
    {
        //services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>( ));
        services.AddControllers( );
        //SERVICES
        services.AddDataProtection( );
        services.AddMemoryCache( );
        services.AddOptions( );


        //FILTERS
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
        services.AddTransient<RequestControl>( );

        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:DataBases"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:GrpcSettings"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:ConfigMongodb"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:LogsPath"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:LoadParameters"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:valida_peticiones_diarias"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:HttpSettings"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:Endpoints"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings:EndpointsAuth"));
        services.Configure<SecurityKeys>(configuration.GetSection("SecurityKeys"));

        return services;
    }
}