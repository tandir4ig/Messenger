using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MessageClient.Extensions;
using MessageClient.HttpClients;
using MessageClient.Models;
using Microsoft.AspNetCore.Components;

namespace MessageClient.Services;

public sealed class AuthorizedHandler(
    ITokenStorageService storage,
    IdentityApiClient identityClient,
    NavigationManager nav)
    : DelegatingHandler
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var access = await storage.GetAccessTokenAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(access))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                access = await storage.GetAccessTokenAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(access))
                {
                    response.Dispose();
                    return await RepeatOriginalAsync(request, access, cancellationToken);
                }

                var refresh = await storage.GetRefreshTokenAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(refresh))
                {
                    await ForceLogoutAsync(cancellationToken);
                    return response;
                }

                /* ► POST /auth/refresh */
                var refreshResp = await identityClient.Client.PostAsJsonAsync(
                    "auth/refresh",
                    new { refreshToken = refresh },
                    cancellationToken);

                if (!refreshResp.IsSuccessStatusCode)
                {
                    await ForceLogoutAsync(cancellationToken);
                    return response;
                }

                var pair = await refreshResp.Content.ReadFromJsonAsync<TokenPair>(cancellationToken);
                await storage.SaveTokenPairAsync(pair!, cancellationToken);

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

    private async Task ForceLogoutAsync(CancellationToken cancellationToken)
    {
        await storage.RemoveTokenPairAsync(cancellationToken);
        nav.NavigateTo("/login", forceLoad: true);    // ← переходим на страницу входа
    }
}
