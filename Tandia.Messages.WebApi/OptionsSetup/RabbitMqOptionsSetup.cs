using Microsoft.Extensions.Options;
using Tandia.Messages.Application.Models;

namespace Tandia.Messages.WebApi.OptionsSetup;

public sealed class RabbitMqOptionsSetup(IConfiguration configuration) : IConfigureOptions<RabbitMqOptions>
{
    private const string SectionName = "RabbitMq";

    public void Configure(RabbitMqOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
