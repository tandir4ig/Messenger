using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Tandia.Identity.Application.Models.Responses;
using Tandia.Identity.WebApi.DTOs.Requests;

namespace Tandia.Identity.ComponentTests;

public sealed class IdentityApiTests : IClassFixture<IdentityIntegrationTestWebFactory>, IAsyncLifetime
{
    private readonly IdentityIntegrationTestWebFactory factory;
    private readonly HttpClient client;

    public IdentityApiTests(IdentityIntegrationTestWebFactory fx)
    {
        factory = fx;
        client = fx.CreateClient();
    }

    [Fact(DisplayName = "Успешная регистрация пользователя")]
    public async Task Register_Returns200()
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new LoginRequest("login", "password"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "Ошибка регистрации при уже существующем email")]
    public async Task Register_DuplicateEmail_Returns400()
    {
        await client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User2", "User2password"));
        var dupe = await client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User2", "User2password"));
        dupe.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Успешный логин")]
    public async Task Login_ReturnsTokens()
    {
        await client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User3", "User3password"));

        var resp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("User3", "User3password"));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokens = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "Логин — пользователь не найден")]
    public async Task Login_UserNotFound_Returns401()
    {
        var resp = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("nouser@test.io", "Pwd1!111111"));

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "Логин — неверный пароль")]
    public async Task Login_WrongPassword_Returns401()
    {
        var user = new RegisterRequest("vasya@test.io", "CorrectPwd1!");

        await client.PostAsJsonAsync("/api/auth/register", user);

        var resp = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(user.Email, "WrongPwd1"));

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "Успешный refresh токен")]
    public async Task Refresh_ReturnsNewTokens()
    {
        await client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User", "Userpassword"));
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("User", "Userpassword"));
        var tokens1 = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

        var resp = await client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest(tokens1!.RefreshToken));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokens2 = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        tokens2!.AccessToken.Should().NotBe(tokens1.AccessToken);
    }

    [Fact(DisplayName = "Ошибка refresh – токен уже использован или просрочен")]
    public async Task Refresh_Reused_Returns400()
    {
        await client.PostAsJsonAsync("/api/auth/register", new LoginRequest("User", "Userpassword"));
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("User", "Userpassword"));
        var tokens1 = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

        await client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest(tokens1!.RefreshToken));
        var second = await client.PostAsJsonAsync("/api/auth/refresh-token", new RefreshTokenRequest(tokens1.RefreshToken));
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---------- IAsyncLifetime ----------
    public Task InitializeAsync() => factory.ResetDbAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
