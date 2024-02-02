using WebUI.Middleware;

using static AccesoDatosGrpcMongo.Neg.DALMongo;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

#region Observability

/// <summary>
/// Observability con OpenTelemetry 
/// jacarrion1 
/// 03/Ene/2024
/// 
/// 
/// Paquetes necesarios:
/// 
/// OpenTelemetry.Exporter.OpenTelemetryProtocol
/// OpenTelemetry.Extensions.Hosting
/// OpenTelemetry.Instrumentation.AspNetCore
/// OpenTelemetry.Instrumentation.Http
/// OpenTelemetry.Instrumentation.Runtime
/// OpenTelemetry.Instrumentation.GrpcNetClient -> descargar con Nuget Package Manager:  PM> NuGet\Install-Package OpenTelemetry.Instrumentation.GrpcNetClient -Version 1.6.0-beta.3
/// OpenTelemetry.Instrumentation.Process -> descargar con Nuget Package Manager:  PM> NuGet\Install-Package OpenTelemetry.Instrumentation.Process -Version 0.5.0-beta.3
/// </summary>
builder.Services.AddOpenTelemetry( )
    .ConfigureResource(resource => resource
        .AddService(serviceName: "wsIdentity"))
    .WithTracing(tracing => tracing
        // trazado de peticiones
        .AddAspNetCoreInstrumentation(o => {
            o.EnrichWithHttpRequest = ( activity, httpRequest ) => {
                activity.SetTag("Date_Request", DateTime.Now);
                Dictionary<string, string> dic_headers = httpRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString( ));
                foreach(var header in dic_headers) {
                    if(string.IsNullOrEmpty(header.Value) ||
                    header.Key.ToLower( ).StartsWith("authorization") ||
                    header.Key.ToLower( ) == "clave-secreta" ||
                    header.Key.ToLower( ) == "secreto") {
                        continue;
                    }
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithHttpResponse = ( activity, httpResponse ) => {
                activity.SetTag("Date_Response", DateTime.Now);
                activity.SetTag("Content_Length2", httpResponse.ContentLength);
                Dictionary<string, string> dic_headers = httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.ToString( ));
                foreach(var header in dic_headers) {
                    if(string.IsNullOrEmpty(header.Value)) {
                        continue;
                    }
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithException = ( activity, exception ) => {
                activity.SetTag("exceptionType", exception.GetType( ).ToString( ));
                activity.SetTag("exceptionMessage", exception.Message);
                activity.SetTag("exceptionStackTrace", exception.StackTrace);
            };
        })
        .AddGrpcClientInstrumentation(o => {
            o.EnrichWithHttpRequestMessage = ( activity, httpRequest ) => {
                activity.SetTag("Date_Request", DateTime.Now);
                Dictionary<string, string> dic_headers = httpRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString( )) ?? new( );
                foreach(var header in dic_headers) {
                    if(string.IsNullOrEmpty(header.Value) ||
                    header.Key.ToLower( ).StartsWith("authorization") ||
                    header.Key.ToLower( ) == "clave-secreta" ||
                    header.Key.ToLower( ) == "secreto") {
                        continue;
                    }
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithHttpResponseMessage = ( activity, httpResponse ) => {
                activity.SetTag("Date_Response", DateTime.Now);
                Dictionary<string, string> dic_headers = httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.ToString( )) ?? new( );
                foreach(var header in dic_headers) {
                    if(string.IsNullOrEmpty(header.Value)) {
                        continue;
                    }
                    activity.SetTag(header.Key, header.Value);
                }
            };
        })
        .AddHttpClientInstrumentation(o => {
            o.EnrichWithHttpRequestMessage = ( activity, httpRequest ) => {
                activity.SetTag("Date_Request", DateTime.Now);
                Dictionary<string, string> dic_headers = httpRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString( )) ?? new( );
                foreach(var header in dic_headers) {
                    if(string.IsNullOrEmpty(header.Value) ||
                    header.Key.ToLower( ).StartsWith("authorization") ||
                    header.Key.ToLower( ) == "clave-secreta" ||
                    header.Key.ToLower( ) == "secreto") {
                        continue;
                    }
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithHttpResponseMessage = ( activity, httpResponse ) => {
                activity.SetTag("Date_Response", DateTime.Now);
                Dictionary<string, string> dic_headers = httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.ToString( )) ?? new( );
                foreach(var header in dic_headers) {
                    if(string.IsNullOrEmpty(header.Value)) {
                        continue;
                    }
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithException = ( activity, exception ) => {
                activity.SetTag("exceptionType", exception.GetType( ).ToString( ));
                activity.SetTag("exceptionMessage", exception.Message);
                activity.SetTag("exceptionStackTrace", exception.StackTrace);
            };
        })
        .AddOtlpExporter(exporter => {
            //TODO: Colocar la URL de donde esucha OpenTelemetry Collector o recuperar dicha URL de forma Dinamica
            exporter.Endpoint = new Uri("http://192.168.55.62:9984");
            exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })).WithMetrics(metric => metric
            // Recoleccion de metricas 
            .AddAspNetCoreInstrumentation( )
            .AddHttpClientInstrumentation( )
            .AddRuntimeInstrumentation( )
            .AddProcessInstrumentation( )
            .AddOtlpExporter(exporter => {
                //TODO: Colocar la URL de donde esucha OpenTelemetry Collector o recuperar dicha URL de forma Dinamica
                exporter.Endpoint = new Uri("http://192.168.55.62:9984");
                exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            }));

#endregion

builder.Services.AddInfrastructureServices( );
builder.Services.AddApplicationServices( );
builder.Services.AddWebUIServices(builder.Configuration);

var grpc = builder.Configuration.GetSection("ApiSettings:GrpcSettings");
var url_mongo = grpc.GetValue<string>("client_grpc_mongo");

builder.Services.AddGrpcClient<DALMongoClient>( o =>
{
    o.Address = new Uri( url_mongo );
} ).ConfigureChannel( c =>
{
    c.HttpHandler = new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
        KeepAlivePingDelay = TimeSpan.FromSeconds(20),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(60),
        EnableMultipleHttp2Connections = true
    };
});


var app = builder.Build( );

app.UseSwagger( );

app.UseSwaggerUI( );

app.UseCors( );

app.UseAuthotizationMego( );

app.UseAuthorization( );

app.UseHttpsRedirection( );

app.MapControllers( );

app.Run( );
