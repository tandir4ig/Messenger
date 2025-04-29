using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Testcontainers.PostgreSql;

namespace Tandia.Identity.ComponentTests;

public sealed class IdentityIntegrationTestWebFactory : WebApplicationFactory<WebApi.Controllers.AuthController>, IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder().WithImage("")
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {

        });
    }
}
