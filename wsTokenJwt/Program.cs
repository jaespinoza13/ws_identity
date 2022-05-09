using Microsoft.OpenApi.Models;
using wsTokenJwt.Middelware;
using wsTokenJwt.Model;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();


//  Swagger/OpenAPI 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Servicio de token Jwt",
        Version = "v1",
        Description = "Servicio de utilidades de la banca virutal CoopMego",
    });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Seguridad b�sica del Api",


    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "basic"
                    }
                },
                Array.Empty<string>()
        }
    });
});

//Configuraci�n de JWT
builder.Services.Configure<SettingsJwt>(builder.Configuration.GetSection("SettingsJwt"));

//Configuraci�n del Servicio 
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:BasicAuth"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:Databases"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:Endpoints"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:PathLogs"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:ConfigAlfresco"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:Config"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:ConfigBdMongo"));
builder.Services.Configure<ServiceSettings>(builder.Configuration.GetSection("ServiceSettings:ParametrosEndpoints"));

builder.Services.Configure<LoadParameters>(builder.Configuration.GetSection("LoadParameters"));

builder.Services.AddOptions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Authorization>();
app.UseMiddleware<RequestControl>();

app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
