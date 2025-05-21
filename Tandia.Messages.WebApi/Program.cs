using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tandia.Messages.WebApi.Consumers;
using Tandia.Messages.WebApi.Extensions;
using Tandia.Messages.WebApi.OptionsSetup;

var builder = WebApplication.CreateBuilder(args);

// CORS (Blazor WASM на https://localhost:7218)
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowBlazorClient",
        builder =>
        {
            builder.WithOrigins("https://localhost:7218") // Укажите URL вашего Blazor WebAssembly приложения
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddMessageServices(
    builder.Configuration.GetConnectionString(
        "DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));

// JWT
builder.Services.AddOptions();
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите: **Bearer {токен}**",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserLoggedInConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // Определяем очередь (endpoint) для получения событий UserLoggedIn
        cfg.ReceiveEndpoint("user-loggedin-queue", e =>
        {
            // Связываем consumer с этой очередью
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
        });
    });
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(rb => rb.AddService("MessageService", serviceVersion: "1.0.0"))
    .WithTracing(tp =>
    {
        tp.AddAspNetCoreInstrumentation();
        tp.AddEntityFrameworkCoreInstrumentation();
        tp.AddHttpClientInstrumentation();
        tp.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        tp.AddOtlpExporter(otlp => otlp.Endpoint = new Uri("http://localhost:4317"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
