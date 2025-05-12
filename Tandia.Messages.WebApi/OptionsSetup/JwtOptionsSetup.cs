using Microsoft.Extensions.Options;

namespace Tandia.Messages.WebApi.OptionsSetup;

public sealed class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtSettings>
{
    private const string SectionName = "Jwt";

    public void Configure(JwtSettings options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
