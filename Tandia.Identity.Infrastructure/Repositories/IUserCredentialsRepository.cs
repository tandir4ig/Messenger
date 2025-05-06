using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public interface IUserCredentialsRepository
{
    Task<UserCredentialsEntity?> GetByIdAsync(Guid id);

    Task<UserCredentialsEntity?> GetByEmailAsync(string email);

    Task<IReadOnlyCollection<UserCredentialsEntity>> GetAllAsync();

    Task AddAsync(UserCredentialsEntity userCredentials);

    Task UpdateAsync(UserCredentialsEntity userCredentials);

    Task DeleteAsync(Guid id);
}
