using Microsoft.Extensions.DependencyInjection;
using Tandia.Identity.Application.Services;
using Tandia.Identity.Application.Services.Interfaces;
using Tandia.Identity.Infrastructure.Models;
using Tandia.Identity.Infrastructure.Repositories;

namespace Tandia.Identity.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IRepository<UserEntity>>(provider => new UserRepository(connectionString));
        services.AddScoped<IRepository<UserCredentialsEntity>>(provider => new UserCredentialsRepository(connectionString));
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
