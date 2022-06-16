using WebUI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices( );
builder.Services.AddApplicationServices( );
builder.Services.AddWebUIServices(builder.Configuration);

var app = builder.Build( );

app.UseSwagger( );

app.UseSwaggerUI( );

app.UseCors( );

app.UseAuthotizationMego( );

app.UseAuthorization( );

app.UseHttpsRedirection( );

app.MapControllers( );

app.Run( );