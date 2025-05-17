namespace Tandia.Identity.Infrastructure.Jobs;

public interface IRefreshTokenCleanupJob
{
    Task CleanupExpiredTokensAsync();
}
