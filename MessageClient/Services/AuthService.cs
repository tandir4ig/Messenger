using System.Net.Http.Json;
using Blazored.LocalStorage;
using MessageClient.Models;

namespace MessageClient.Services;

public sealed class AuthService(IHttpClientFactory factory, ILocalStorageService localStorage)
{
    private readonly HttpClient httpClient = factory.CreateClient("IdentityApi");

    public async Task<bool> LoginAsync(LoginRequest model)
    {
        var resp = await httpClient.PostAsJsonAsync("/api/auth/login", model);
        if (!resp.IsSuccessStatusCode)
        {
            return false;
        }

        var tokens = await resp.Content.ReadFromJsonAsync<TokenPair>();
        await localStorage.SetItemAsync("access_token", tokens!.AccessToken);
        await localStorage.SetItemAsync("refresh_token", tokens.RefreshToken);
        return true;
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync("access_token");
        await localStorage.RemoveItemAsync("refresh_token");
    }

    public async Task<string?> GetAccessTokenAsync() =>
        await localStorage.GetItemAsync<string>("access_token");

    public async Task<bool> TryRefreshAsync()
    {
        var refresh = await localStorage.GetItemAsync<string>("refresh_token");
        if (string.IsNullOrWhiteSpace(refresh))
        {
            return false;
        }

        var resp = await httpClient.PostAsJsonAsync("auth/refresh", new { refreshToken = refresh });
        if (!resp.IsSuccessStatusCode)
        {
            return false;
        }

        var tokens = await resp.Content.ReadFromJsonAsync<TokenPair>();
        await localStorage.SetItemAsync("access_token", tokens!.AccessToken);
        await localStorage.SetItemAsync("refresh_token", tokens.RefreshToken);
        return true;
    }
}
