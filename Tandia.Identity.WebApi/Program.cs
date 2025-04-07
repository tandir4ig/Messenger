using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tandia.Identity.Application.Models;
using Tandia.Identity.WebApi.Extensions;
using Tandia.Identity.WebApi.OptionsSetup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddIdentityServices(
    builder.Configuration.GetConnectionString(
        "DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
    builder.Configuration.GetSection("JwtSettings").Bind);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
