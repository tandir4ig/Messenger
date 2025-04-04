using Tandia.Identity.Application.Extensions;
using Tandia.Identity.Infrastructure.Services;

namespace Tandia.Identity.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, string connectionString)
    {
        services.AddHostedService<MigrationService>();
        services.AddUsers(connectionString);

        return services;
    }
}
