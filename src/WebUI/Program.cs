using WebUI.Middleware;
using static AccesoDatosGrpcAse.Neg.DAL;
using static AccesoDatosGrpcMongo.Neg.DALMongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices( );
builder.Services.AddApplicationServices( );
builder.Services.AddWebUIServices(builder.Configuration);

var grpc = builder.Configuration.GetSection("ApiSettings:GrpcSettings");
var url_sybase = grpc.GetValue<string>("client_grpc_sybase");
var url_mongo = grpc.GetValue<string>("client_grpc_mongo");
builder.Services.AddGrpcClient<DALClient>(o =>
{
    o.Address = new Uri(url_sybase);
}).ConfigureChannel(c =>
{
    c.HttpHandler = new SocketsHttpHandler
    {
        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
        KeepAlivePingDelay = TimeSpan.FromSeconds(20),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(60),
        EnableMultipleHttp2Connections = true
    };
});
builder.Services.AddGrpcClient<DALMongoClient>(o =>
{
    o.Address = new Uri(url_mongo);
}).ConfigureChannel(c =>
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
