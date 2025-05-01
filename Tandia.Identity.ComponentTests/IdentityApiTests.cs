using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
        dupe.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ---------- IAsyncLifetime ----------
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => factory.ResetDatabaseAsync();
}
