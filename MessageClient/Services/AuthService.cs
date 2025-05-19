using System.Net.Http.Json;
using MessageClient.HttpClients;
using MessageClient.Models;
using Microsoft.AspNetCore.Components;

namespace MessageClient.Services;

public sealed class AuthService(IdentityApiClient identityClient, ITokenStorageService tokenStorage, NavigationManager navigator)
{
    private HttpClient HttpClient => identityClient.Client;

    public async Task<bool> LoginAsync(LoginRequest model)
    {
        var resp = await HttpClient.PostAsJsonAsync("/api/auth/login", model);
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

        var resp = await HttpClient.PostAsJsonAsync("auth/refresh", new { refreshToken = refresh });
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
