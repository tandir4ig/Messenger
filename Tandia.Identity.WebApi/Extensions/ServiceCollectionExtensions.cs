using Tandia.Identity.Application.Extensions;
using Tandia.Identity.Infrastructure.Services;

namespace Tandia.Identity.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddHostedService<MigrationService>();
        services.AddUsers();

        return services;
    }
}
