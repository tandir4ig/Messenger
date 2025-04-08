using Microsoft.Extensions.Options;
using Tandia.Identity.Application.Models;

namespace Tandia.Identity.WebApi.OptionsSetup;

public sealed class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtSettings>
{
    private const string SectionName = "Jwt";

    public void Configure(JwtSettings options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
