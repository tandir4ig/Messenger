using Hangfire;
using Microsoft.Extensions.Hosting;
using Tandia.Identity.Infrastructure.Jobs;

namespace Tandia.Identity.Infrastructure.Services;

public sealed class RecurringJobsHostedService(IRecurringJobManager recurringJobManager) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        recurringJobManager.AddOrUpdate<IRefreshTokenCleanupJob>(
                   "cleanup-expired-refresh-tokens",
                   job => job.CleanupExpiredTokensAsync(),
                   cronExpression: "*/5 * * * *");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
