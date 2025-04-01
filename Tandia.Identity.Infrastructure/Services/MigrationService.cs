using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Tandia.Identity.Infrastructure.Migrations;

namespace Tandia.Identity.Infrastructure.Services;

public sealed class MigrationService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var connection = new NpgsqlConnection("Host=localhost;port=5432;Database=Messenger;Username=postgres;Password=Fiit2002@)");
        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(MigrationScripts.InitialMigration);
    }

    public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}
