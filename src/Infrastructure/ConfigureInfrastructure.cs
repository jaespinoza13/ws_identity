
using Application.Common.Interfaces;
using Infrastructure.DailyRequest;
using Infrastructure.gGRPC_Clients.Mongo;
using Infrastructure.gRPC_Clients.Sybase;
using Infrastructure.Services;
using Infrastructure.MemoryCache;
using Infrastructure.ExternalAPIs;
using Infrastructure.Common.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureInfrastructure
{
    public static IServiceCollection AddInfrastructureServices ( this IServiceCollection services )
    {
        //INFRAESTRUCTURA
        services.AddSingleton<ParametersInMemory>( );
        services.AddSingleton<ILogs, LogsService>( );
        services.AddSingleton<IMongoDat, LogsMongoDat>( );
        services.AddSingleton<IDailyRequest, DailyRequest>( );
        services.AddSingleton<IParametersInMemory, ParametersInMemory>( );
        services.AddSingleton<IParametrosDat, ParametrosDat>( );
        services.AddTransient<IHttpService, HttpService>( );
        services.AddSingleton<IAutenticarseDat, AutenticarseDat>( );
        services.AddTransient<IEncryptMego, EncryptMego>( );
        services.AddTransient<IWsOtp, WsOtp>( );
        services.AddSingleton<IOtpDat, OtpDat>( );

        //CASOS DE USO
        services.AddSingleton<IAccesoDat, RecuperarContraseniaDat>( );


        return services;
    }
}
