using System.Text.Json.Serialization;
using VegetableLine.SimulationSettings;
using VegetableLine.Simulation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Los enums se devuelven como texto ("Running") en lugar de su numero
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Orígenes permitidos, leídos de la configuración (en Render se pueden sobrescribir con variables de entorno)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddSingleton<SimulationSettings>();
builder.Services.AddSingleton<SimulationContext>();
builder.Services.AddHostedService<LineSimulator>();
builder.Services.AddSingleton<AlarmMonitor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapControllers();

app.Run();
