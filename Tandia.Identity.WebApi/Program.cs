using Hangfire;
using Hangfire.Redis.StackExchange;
using MassTransit;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tandia.Identity.WebApi.Extensions;
using Tandia.Identity.WebApi.OptionsSetup;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// CORS
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

builder.Services.AddIdentityServices();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// JWT
builder.Services.AddOptions();
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.ConfigureOptions<DatabaseOptionsSetup>();
builder.Services.ConfigureOptions<RabbitMqOptionsSetup>();

// Hangfire background services
builder.Services.AddHangfire(hf => hf
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseRedisStorage(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddHangfireServer();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(rb => rb.AddService("IdentityService", serviceVersion: "1.0.0"))
    .WithTracing(tp =>
    {
        tp.AddAspNetCoreInstrumentation();
        tp.AddHttpClientInstrumentation();
        tp.AddSqlClientInstrumentation(opts => opts.SetDbStatementForText = true);
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

app.UseHangfireDashboard();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorClient");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
