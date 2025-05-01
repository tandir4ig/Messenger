using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tandia.Identity.Application.Models.Responses;
using Tandia.Identity.WebApi.DTOs.Requests;

namespace Tandia.Identity.ComponentTests;

public sealed class IdentityApiTests : IClassFixture<IdentityIntegrationTestWebFactory>, IAsyncLifetime
{
    private readonly IdentityIntegrationTestWebFactory factory;
    private readonly HttpClient _client;

    public IdentityApiTests(IdentityIntegrationTestWebFactory fx)
    {
        factory = fx;
        _client = fx.CreateClient();
    }

    [Fact(DisplayName = "Успешная регистрация пользователя")]
    public async Task Register_Returns200()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest("login", "password"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Ошибка регистрации при уже существующем email")]
    public async Task Register_DuplicateEmail_Returns400()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User2", "User2password"));
        var dupe = await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User2", "User2password"));
        dupe.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Успешный логин")]
    public async Task Login_ReturnsTokens()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User3", "User3password"));

        var resp = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("User3", "User3password"));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokens = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Theory(DisplayName = "Ошибка логина – пользователь не найден или неверный пароль")]
    [InlineData("user1@test", "user1Password")]
    [InlineData("user2@test", "user2Password")]
    public async Task Login_Fails_With401(string email, string pwd)
    {
        // если проверяем неверный пароль, нужно сначала создать пользователя
        if (email == "user2@test")
        {
            await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest(email, "wrongPassword"));
        }

        var resp = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, pwd));
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "Успешный refresh токен")]
    public async Task Refresh_ReturnsNewTokens()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User", "Userpassword"));
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("User", "Userpassword"));
        var tokens1 = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

        await Task.Delay(1100);

        var resp = await _client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest(tokens1!.RefreshToken));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokens2 = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        tokens2!.AccessToken.Should().NotBe(tokens1.AccessToken);
    }

    [Fact(DisplayName = "Ошибка refresh – токен уже использован или просрочен")]
    public async Task Refresh_Reused_Returns400()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User", "Userpassword"));
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("User", "Userpassword"));
        var tokens1 = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

        await _client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest(tokens1!.RefreshToken));
        var second = await _client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest(tokens1.RefreshToken));
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---------- IAsyncLifetime ----------
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => factory.ResetDbAsync();
}
