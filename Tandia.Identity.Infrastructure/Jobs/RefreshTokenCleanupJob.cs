using Microsoft.Extensions.Logging;
using Tandia.Identity.Infrastructure.Repositories;

namespace Tandia.Identity.Infrastructure.Jobs;

public sealed class RefreshTokenCleanupJob(
    IRefreshTokenRepository refreshTokenRepository,
    TimeProvider timeProvider,
    ILogger<RefreshTokenCleanupJob> log)
    : IRefreshTokenCleanupJob
{
    public async Task CleanupExpiredTokensAsync()
    {
        await refreshTokenRepository.DeleteExpiredTokensAsync(timeProvider.GetUtcNow());

        log.LogInformation("Refresh-cleanup");
    }
}
