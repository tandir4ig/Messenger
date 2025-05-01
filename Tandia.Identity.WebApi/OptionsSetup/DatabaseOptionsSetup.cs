using Microsoft.Extensions.Options;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.WebApi.OptionsSetup;

public sealed class DatabaseOptionsSetup(IConfiguration configuration) : IConfigureOptions<DatabaseOptions>
{
    private const string SectionName = "ConnectionStrings";

    public void Configure(DatabaseOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
