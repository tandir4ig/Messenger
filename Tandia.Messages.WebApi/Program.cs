namespace MessageApi;

using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Application.Extensions;
using Tandia.Messages.Infrastructure.Data;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
        builder.Services.AddBusinessLogicServices(builder.Configuration.GetConnectionString("DefaultConnection"));

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowBlazorClient");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
