using Tandia.Identity.Infrastructure.Jobs;
using Tandia.Identity.Infrastructure.Services;

namespace Tandia.Identity.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IRefreshTokenCleanupJob, RefreshTokenCleanupJob>();
        services.AddHostedService<MigrationService>();
        services.AddHostedService<RecurringJobsHostedService>();
        services.AddUsers();

        return services;
    }
}
