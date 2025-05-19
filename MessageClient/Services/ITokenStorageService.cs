using MessageClient.Models;

namespace MessageClient.Services;

public interface ITokenStorageService
{
    Task<TokenPair?> GetTokenPairAsync(CancellationToken ct = default);

    Task SaveTokenPairAsync(TokenPair pair, CancellationToken ct = default);

    Task RemoveTokenPairAsync(CancellationToken ct = default);

    Task<string?> GetAccessTokenAsync(CancellationToken ct = default);

    Task<string?> GetRefreshTokenAsync(CancellationToken ct = default);
}
