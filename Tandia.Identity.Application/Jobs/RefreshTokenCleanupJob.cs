using Microsoft.Extensions.Logging;
using Tandia.Identity.Infrastructure.Repositories;

namespace Tandia.Identity.Application.Jobs;

public sealed class RefreshTokenCleanupJob(
    IRefreshTokenRepository refreshTokenRepository,
    TimeProvider timeProvider,
    ILogger<RefreshTokenCleanupJob> log)
{
    public async Task ExecuteAsync()
    {
        await refreshTokenRepository.DeleteExpiredTokensAsync(timeProvider.GetUtcNow());

        log.LogInformation("Refresh-cleanup");
    }
}
