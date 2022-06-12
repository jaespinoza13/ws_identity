using WebUI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices( );
builder.Services.AddApplicationServices( );
builder.Services.AddWebUIServices(builder.Configuration);


var AllowSpecificOrigins = "_AllowSpecificOrigins";




var app = builder.Build( );

if (app.Environment.IsDevelopment( ))
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseCors(AllowSpecificOrigins);

app.UseAuthotizationMego( );

app.UseAuthorization( );

app.UseHttpsRedirection( );

app.MapControllers( );

app.Run( );