using Blazored.LocalStorage;
using MessageClient.Models;

namespace MessageClient.Services;

public sealed class LocalStorageTokenService(ILocalStorageService localStorage) : ITokenStorageService
{
    private const string AccessKey = "access_token";
    private const string RefreshKey = "refresh_token";

    public async Task<TokenPair?> GetTokenPairAsync(CancellationToken ct = default)
    {
        var access = await localStorage.GetItemAsync<string>(AccessKey, ct);
        var refresh = await localStorage.GetItemAsync<string>(RefreshKey, ct);

        if (string.IsNullOrWhiteSpace(access) || string.IsNullOrWhiteSpace(refresh))
        {
            return null;
        }

        return new TokenPair(access, refresh);
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken ct = default) =>
        await localStorage.GetItemAsync<string>(AccessKey, ct);

    public async Task<string?> GetRefreshTokenAsync(CancellationToken ct = default) =>
        await localStorage.GetItemAsync<string>(RefreshKey, ct);

    public async Task SaveTokenPairAsync(TokenPair pair, CancellationToken ct = default)
    {
        await localStorage.SetItemAsync(AccessKey, pair.AccessToken, ct);
        await localStorage.SetItemAsync(RefreshKey, pair.RefreshToken, ct);
    }

    public async Task RemoveTokenPairAsync(CancellationToken ct = default)
    {
        await localStorage.RemoveItemAsync(AccessKey, ct);
        await localStorage.RemoveItemAsync(RefreshKey, ct);
    }
}
