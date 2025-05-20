using Hangfire;
using Hangfire.Redis.StackExchange;
using MassTransit;
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

// Hangfire background services
builder.Services.AddHangfire(hf => hf
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseRedisStorage(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddHangfireServer();
builder.Services.AddHangfireServer();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
    // В этом сервисе у нас только публикация, потребителей не регистрируем.
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
