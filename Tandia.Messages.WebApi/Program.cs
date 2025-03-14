using Tandia.Messages;
using Tandia.Messages.Services;
using Tandia.Messages.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7218") // Укажите URL вашего Blazor WebAssembly приложения
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Add services to the container.
            builder.Services.AddTransient<IMessageRepository, MessageRepository>();

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                db.Database.Migrate();
            }

            app.UseCors("AllowBlazorClient");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
