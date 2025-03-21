namespace Tandia.Messages.ComponentTests;

using MessageApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tandia.Messages.Infrastructure.Data;

public class TandiaWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var DbContext = services.SingleOrDefault(
                context => context.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (DbContext != null)
            {
                services.Remove(DbContext);
            }

            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("DbInMemory"));

            var sp = services.BuildServiceProvider();
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
