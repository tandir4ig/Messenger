using System.Net.Http.Json;
using MessageClient.Models;
using Microsoft.AspNetCore.Components;

namespace MessageClient.Services;

public sealed class AuthService(IHttpClientFactory factory, ITokenStorageService tokenStorage, NavigationManager navigator)
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
        if (tokens == null)
        {
            return false;
        }

        await tokenStorage.SaveTokenPairAsync(tokens);
        return true;
    }

    public async Task LogoutAsync()
    {
        await tokenStorage.RemoveTokenPairAsync();
        navigator.NavigateTo("/login", forceLoad: true);
    }

    public async Task<string?> GetAccessTokenAsync() => await tokenStorage.GetAccessTokenAsync();

    public async Task<bool> TryRefreshAsync()
    {
        var refresh = await tokenStorage.GetRefreshTokenAsync();
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
        if (tokens is null)
        {
            return false;
        }

        await tokenStorage.SaveTokenPairAsync(tokens);
        return true;
    }
}
