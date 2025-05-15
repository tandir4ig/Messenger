using Hangfire;
using Tandia.Identity.Application.Jobs;

namespace Tandia.Identity.WebApi.Extensions;

public static class BackgroundJobsExtensions
{
    public static void RegisterBackgroundJobs(this IApplicationBuilder app)
    {
        app.ApplicationServices.GetRequiredService<IRecurringJobManager>()
           .AddOrUpdate<RefreshTokenCleanupJob>(
               "cleanup-expired-refresh-tokens",
               job => job.ExecuteAsync(),
               cronExpression: "*/5 * * * *");
    }
}
