using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tandia.Messages.WebApi.OptionsSetup;

public sealed class JwtBearerOptionsSetup(IOptions<JwtSettings> jwtOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    // вызывется, когда запрашивают опции С КОНКРЕТНЫМ именем схемы
    public void Configure(string? name, JwtBearerOptions options)
    {
        // применяем настройки только к схеме "Bearer"
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        Apply(options);
    }

    // вызывается, когда запрашивают опции БЕЗ имени
    public void Configure(JwtBearerOptions options) => Apply(options);

    // конфигурация токена
    private void Apply(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidAudience = jwtOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)),
        };
    }
}
