using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Tandia.Identity.Infrastructure.Migrations;
using Tandia.Identity.Infrastructure.Models;
using Testcontainers.PostgreSql;

namespace Tandia.Identity.ComponentTests;

public sealed class IdentityIntegrationTestWebFactory : WebApplicationFactory<WebApi.Controllers.AuthController>, IAsyncLifetime
{
    private PostgreSqlContainer _container = default!;
    private Respawner _respawner = default!;

    public string ConnectionString => _container.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<DatabaseOptions>(opt => opt.ConnectionString = ConnectionString);
        });
    }

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("app")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:16-alpine")
            .Build();
        await _container.StartAsync();

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await conn.ExecuteAsync(MigrationScripts.InitialMigration);

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude =
            ["public"],
        });
    }

    public new async Task DisposeAsync() => await _container.DisposeAsync();

    public async Task ResetDbAsync()
    {
        await using var resetConn = new NpgsqlConnection(ConnectionString);
        await resetConn.OpenAsync();
        await _respawner.ResetAsync(resetConn);
    }
}
