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
});
builder.Services.AddGrpcClient<DALMongoClient>(o =>
{
    o.Address = new Uri(url_mongo);
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