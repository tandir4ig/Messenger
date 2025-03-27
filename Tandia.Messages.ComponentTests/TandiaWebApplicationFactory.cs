using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Tandia.Messages.Infrastructure.Data;
using Tandia.Messages.Infrastructure.Services;

namespace Tandia.Messages.ComponentTests;

public sealed class TandiaWebApplicationFactory : WebApplicationFactory<WebApi.Controllers.MessagesController>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(
                context => context.ServiceType == typeof(IDbContextOptionsConfiguration<DatabaseContext>));

            var service = services.SingleOrDefault(
                c => c.ImplementationType == typeof(DatabaseMigrationService));

            if (service != null)
            {
                services.Remove(service);
            }

            if (dbContext != null)
            {
                services.Remove(dbContext);
            }

            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("DbInMemory"));
        });
    }

    public async Task ResetAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
