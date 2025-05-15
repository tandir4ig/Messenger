using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using MessageClient.Extensions;
using MessageClient.Models;
using Microsoft.AspNetCore.Components;

namespace MessageClient.Services;

public sealed class AuthorizedHandler : DelegatingHandler
{
    private readonly ILocalStorageService _storage;
    private readonly IHttpClientFactory _factory;
    private readonly NavigationManager _nav;
    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthorizedHandler(ILocalStorageService storage,
                             IHttpClientFactory factory,
                             NavigationManager nav)
    {
        _storage = storage;
        _factory = factory;
        _nav = nav;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var access = await _storage.GetItemAsync<string>("access_token", cancellationToken);

        if (!string.IsNullOrWhiteSpace(access))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized &&
            !request.RequestUri!.AbsolutePath.Contains("/auth/", StringComparison.OrdinalIgnoreCase))
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                access = await _storage.GetItemAsync<string>("access_token", cancellationToken);

                if (!string.IsNullOrWhiteSpace(access))
                {
                    response.Dispose();
                    return await RepeatOriginalAsync(request, access, cancellationToken);
                }

                var refresh = await _storage.GetItemAsync<string>("refresh_token", cancellationToken);
                if (string.IsNullOrWhiteSpace(refresh))
                {
                    await ForceLogoutAsync(cancellationToken);
                    return response;
                }

                /* ► POST /auth/refresh */
                var client = _factory.CreateClient("IdentityApi"); // клиент без Bearer
                var refreshResp = await client.PostAsJsonAsync("auth/refresh",
                                            new { refreshToken = refresh }, cancellationToken);

                if (!refreshResp.IsSuccessStatusCode)
                {
                    await ForceLogoutAsync(cancellationToken);
                    return response;
                }

                var pair = await refreshResp.Content.ReadFromJsonAsync<TokenPair>(cancellationToken);
                await SaveTokensAsync(pair!);

                response.Dispose();
                return await RepeatOriginalAsync(request, pair!.AccessToken, cancellationToken);
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        return response;
    }

    private Task<HttpResponseMessage> RepeatOriginalAsync(
        HttpRequestMessage original, string newAccess, CancellationToken ct)
    {
        var clone = original.Clone();
        clone.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccess);
        return base.SendAsync(clone, ct);
    }

    private async Task SaveTokensAsync(TokenPair pair)
    {
        await _storage.SetItemAsync("access_token", pair.AccessToken);
        await _storage.SetItemAsync("refresh_token", pair.RefreshToken);
    }

    private async Task ForceLogoutAsync(CancellationToken ct)
    {
        await _storage.RemoveItemAsync("access_token", ct);
        await _storage.RemoveItemAsync("refresh_token", ct);
        _nav.NavigateTo("/login", forceLoad: true);    // ← переходим на страницу входа
    }
}
