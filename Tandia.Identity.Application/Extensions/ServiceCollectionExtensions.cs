using Microsoft.Extensions.DependencyInjection;
using Tandia.Identity.Application.Services;
using Tandia.Identity.Application.Services.Interfaces;
using Tandia.Identity.Infrastructure.Repositories;

namespace Tandia.Identity.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(
        this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
