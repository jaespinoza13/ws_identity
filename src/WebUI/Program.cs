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
/// 15/Mar/2024
/// 
/// 
/// Paquetes NuGet necesarios:
/// 
/// OpenTelemetry
/// OpenTelemetry.Exporter.OpenTelemetryProtocol
/// OpenTelemetry.Extensions.Hosting
/// OpenTelemetry.Instrumentation.AspNetCore
/// OpenTelemetry.Instrumentation.Http
/// OpenTelemetry.Instrumentation.Runtime
/// OpenTelemetry.Instrumentation.GrpcNetClient -> descargar con Nuget Package Manager:  PM> NuGet\Install-Package OpenTelemetry.Instrumentation.GrpcNetClient -Version 1.6.0-beta.3
/// OpenTelemetry.Instrumentation.Process -> descargar con Nuget Package Manager:  PM> NuGet\Install-Package OpenTelemetry.Instrumentation.Process -Version 0.5.0-beta.3
/// </summary>
String instance_name = builder.Configuration.GetSection("Config:OTLP").GetValue<string>("instance_name") ?? "wsIdentity";
String? url_otel_access = builder.Configuration.GetSection("Config:OTLP").GetValue<string>("url_otel_access");

builder.Services.AddOpenTelemetry( )
    .ConfigureResource(resource => resource
        .AddService(serviceName: instance_name))
    .WithTracing(tracing => tracing
        // trazado de peticiones
        .AddAspNetCoreInstrumentation(o => {
            o.EnrichWithHttpRequest = ( activity, httpRequest ) => {
                activity.SetTag("Asp_Date_Request", DateTime.Now);
                foreach(var header in obtenerHeaders(null, httpRequest.Headers)) {
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithHttpResponse = ( activity, httpResponse ) => {
                activity.SetTag("Asp_Date_Response", DateTime.Now);
                activity.SetTag("Content_Length2", httpResponse.ContentLength);
                foreach(var header in obtenerHeaders(null, httpResponse.Headers)) {
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithException = ( activity, exception ) => {
                activity.SetTag("Asp_Exception_Type", exception.GetType( ).ToString( ));
                activity.SetTag("Asp_Exception_Message", exception.Message);
                activity.SetTag("Asp_Exception_Stack_Trace", exception.StackTrace);
            };
        })
        .AddGrpcClientInstrumentation(o => {
            o.EnrichWithHttpRequestMessage = ( activity, httpRequest ) => {
                activity.SetTag("gRPC_Date_Request", DateTime.Now);
                activity.SetTag("gRPC_Request_Uri", httpRequest.RequestUri);
                activity.SetTag("gRPC_Request_Version", httpRequest.Version);
                activity.SetTag("gRPC_Request_Version_Policy", httpRequest.VersionPolicy);
                foreach(var header in obtenerHeaders(httpRequest.Headers, null)) {
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithHttpResponseMessage = ( activity, httpResponse ) => {
                activity.SetTag("gRPC_Date_Response", DateTime.Now);
                if(!String.IsNullOrWhiteSpace(httpResponse.ReasonPhrase)) {
                    activity.SetTag("gRPC_Reason_Phrase", httpResponse.ReasonPhrase);
                }
                activity.SetTag("gRPC_Response_Version", httpResponse.Version);
                foreach(var header in obtenerHeaders(httpResponse.TrailingHeaders, null)) {
                    activity.SetTag("Trailing_" + header.Key, header.Value);
                }
                foreach(var header in obtenerHeaders(httpResponse.Headers, null)) {
                    activity.SetTag(header.Key, header.Value);
                }
            };
        })
        .AddHttpClientInstrumentation(o => {
            o.EnrichWithHttpRequestMessage = ( activity, httpRequest ) => {
                activity.SetTag("Http_Date_Request", DateTime.Now);
                foreach(var header in obtenerHeaders(httpRequest.Headers, null)) {
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithHttpResponseMessage = ( activity, httpResponse ) => {
                activity.SetTag("Http_Date_Response", DateTime.Now);
                foreach(var header in obtenerHeaders(httpResponse.Headers, null)) {
                    activity.SetTag(header.Key, header.Value);
                }
            };
            o.EnrichWithException = ( activity, exception ) => {
                activity.SetTag("Http_Exception_Type", exception.GetType( ).ToString( ));
                activity.SetTag("Http_Exception_Message", exception.Message);
                activity.SetTag("Http_Exception_Stack_Trace", exception.StackTrace);
            };
        })
        .AddOtlpExporter(exporter => {
            if(!String.IsNullOrWhiteSpace(url_otel_access)) {
                exporter.Endpoint = new Uri(url_otel_access);
                exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            }
        }))
    .WithMetrics(metric => metric
        // Recoleccion de metricas 
        .AddAspNetCoreInstrumentation( )
        .AddHttpClientInstrumentation( )
        .AddRuntimeInstrumentation( )
        .AddProcessInstrumentation( )
        .AddOtlpExporter(exporter => {
            //TODO: Colocar la URL de donde esucha OpenTelemetry Collector o recuperar dicha URL de forma Dinamica
            if(!String.IsNullOrWhiteSpace(url_otel_access)) {
                exporter.Endpoint = new Uri(url_otel_access);
                exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            }
        }));

Dictionary<string, string> obtenerHeaders ( System.Net.Http.Headers.HttpHeaders? headers1, IHeaderDictionary? headers2 ) {
    if(headers1 != null) {
        return headers1.ToDictionary(x => x.Key, x => descomponerHeader(x.Key, x.Value.ToList( ))) ?? new( );
    }
    if(headers2 != null) {
        return headers2.ToDictionary(x => x.Key, x => descomponerHeader(x.Key, x.Value.ToList( ))) ?? new( );
    }
    return new( );
}

String descomponerHeader ( String key, List<String?> value ) {
    if(key.ToLower( ).StartsWith("authorization") ||
                   key.ToLower( ) == "clave-secreta" ||
                   key.ToLower( ) == "secreto") {
        return "**************";
    } else {
        String valores = "";
        value.ForEach(( str ) => {
            if(!String.IsNullOrWhiteSpace(str)) {
                valores += str + " | ";
            }
        });
        return valores.EndsWith(" | ") ? valores.Substring(0, valores.Length - 3) : "No Data";
    }
}

#endregion

builder.Services.AddInfrastructureServices( );
builder.Services.AddApplicationServices( );
builder.Services.AddWebUIServices(builder.Configuration);

var grpc = builder.Configuration.GetSection("ApiSettings:GrpcSettings");
var url_mongo = grpc.GetValue<string>("client_grpc_mongo");

builder.Services.AddGrpcClient<DALMongoClient>(o => {
    o.Address = new Uri(url_mongo);
}).ConfigureChannel(c => {
    c.HttpHandler = new SocketsHttpHandler {
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
