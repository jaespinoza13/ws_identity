
using Application.Common.Interfaces;
using Infrastructure.DailyRequest;
using Infrastructure.gGRPC_Clients.Mongo;
using Infrastructure.gRPC_Clients.Sybase;
using Infrastructure.Services;
using Infrastructure.MemoryCache;
using Infrastructure.ExternalAPIs;
using Infrastructure.Common.Interfaces;
using Application.RecuperarReenvio;
using Application.LogInMegomovil;

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
        services.AddTransient<IParametrosDat, ParametrosDat>( );
        services.AddSingleton<IHttpService, HttpService>( );
        services.AddTransient<IAutenticarseDat, AutenticarseDat>( );
        services.AddSingleton<IEncryptMego, EncryptMego>( );
        services.AddSingleton<IWsOtp, WsOtp>( );
        services.AddTransient<IOtpDat, OtpDat>( );
        services.AddTransient<IKeysDat , KeysDat>( );
        services.AddTransient<IAutenticarseMegomovilDat, AutenticarseMegomovilDat>( );
        services.AddTransient<ICifradoMegomovil, CifradoMegomovil>( );
        services.AddTransient<IKeysMovilDat, KeysMovilDat>( );

        //CASOS DE USO
        services.AddTransient<IAccesoDat, RecuperarContraseniaDat>( );
        services.AddTransient<IValidarRecuperaciones, ValidarRecuperacionesDat>( );
        services.AddTransient<IAutenticarseUsuarioExternoDat, AutenticarseUsuarioExternoDat>( );

        return services;
    }
}
