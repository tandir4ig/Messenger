using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
builder.Services.ConfigureOptions<RabbitMqOptionsSetup>();

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

builder.Services.AddOpenTelemetry()
    .ConfigureResource(rb => rb.AddService("MessageService", serviceVersion: "1.0.0"))
    .WithTracing(tp =>
    {
        tp.AddAspNetCoreInstrumentation();
        tp.AddEntityFrameworkCoreInstrumentation();
        tp.AddHttpClientInstrumentation();
        tp.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        tp.AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(
                builder.Configuration.GetConnectionString("OtlpEndpoint")
                ?? throw new InvalidOperationException("Connection string 'OtlpEndpoint' not found."));
            opts.Protocol = OtlpExportProtocol.Grpc;
        });
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
