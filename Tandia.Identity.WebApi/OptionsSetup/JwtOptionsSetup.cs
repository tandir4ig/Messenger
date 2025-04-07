using Microsoft.Extensions.Options;
using Tandia.Identity.Application.Models;

namespace Tandia.Identity.WebApi.OptionsSetup;

public sealed class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "Jwt";

    public void Configure(JwtOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
