using Microsoft.Extensions.Options;
using Tandia.Identity.Application.Models;

namespace Tandia.Identity.WebApi.OptionsSetup;

public sealed class RabbitMqOptionsSetup(IConfiguration configuration) : IConfigureOptions<RabbitMqOptions>
{
    private const string SectionName = "RabbitMq";

    public void Configure(RabbitMqOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
