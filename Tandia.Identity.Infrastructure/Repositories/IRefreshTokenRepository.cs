namespace Tandia.Identity.Infrastructure.Repositories;

public interface IRefreshTokenRepository
{
    Task UpdateRefreshTokenAsync(Guid id, string refreshToken);
}
