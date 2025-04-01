using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tandia.Messages.Infrastructure.Data;
using Tandia.Messages.Infrastructure.Services;

namespace Tandia.Messages.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DatabaseContext>(options =>
                        options.UseNpgsql(connectionString));

        services.AddHostedService<DatabaseMigrationService>();

        return services;
    }
}
