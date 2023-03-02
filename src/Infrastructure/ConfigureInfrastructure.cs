
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
        services.AddSingleton<IParametrosDat, ParametrosDat>( );
        services.AddSingleton<IHttpService, HttpService>( );
        services.AddSingleton<IAutenticarseDat, AutenticarseDat>( );
        services.AddSingleton<IEncryptMego, EncryptMego>( );
        services.AddSingleton<IWsOtp, WsOtp>( );
        services.AddSingleton<IOtpDat, OtpDat>( );
        services.AddSingleton<IKeysDat , KeysDat>( );
        services.AddSingleton<IAutenticarseMegomovilDat, AutenticarseMegomovilDat>( );
        services.AddSingleton<ICifradoMegomovil, CifradoMegomovil>( );
        services.AddSingleton<IKeysMegomovilDat, KeysMegomovilDat>( );

        //CASOS DE USO
        services.AddSingleton<IAccesoDat, RecuperarContraseniaDat>( );
        services.AddSingleton<IValidarRecuperaciones, ValidarRecuperacionesDat>( );

        return services;
    }
}
