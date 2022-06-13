﻿
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
        services.AddTransient<ParametersInMemory>( );
        services.AddTransient<ILogs, LogsService>( );
        services.AddTransient<IMongoDat, LogsMongoDat>( );
        services.AddTransient<IDailyRequest, DailyRequest>( );
        services.AddTransient<IParametersInMemory, ParametersInMemory>( );
        services.AddTransient<IParametrosDat, ParametrosDat>( );
        services.AddTransient<IHttpService, HttpService>( );
        services.AddTransient<IAutenticarseDat, AutenticarseDat>( );
        services.AddTransient<IEncryptMego, EncryptMego>( );
        services.AddTransient<IWsOtp, WsOtp>( );
        services.AddTransient<IOtpDat, OtpDat>( );

        //CASOS DE USO
        services.AddTransient<IEncryptMego, EncryptMego>( );
        services.AddTransient<IAccesoDat, RecuperarContraseniaDat>( );


        return services;
    }
}
