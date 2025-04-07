using Tandia.Identity.Application.Extensions;
using Tandia.Identity.Application.Models;
using Tandia.Identity.Infrastructure.Services;

namespace Tandia.Identity.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, string connectionString, Action<JwtOptions> config)
    {
        services.AddHostedService<MigrationService>();
        services.AddUsers(connectionString, config);

        return services;
    }
}
