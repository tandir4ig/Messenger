using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id);

    Task<UserEntity?> GetByEmailAsync(string email);

    Task<IReadOnlyCollection<UserEntity>> GetAllAsync();

    Task AddAsync(UserEntity user);

    Task UpdateAsync(UserEntity user);

    Task DeleteAsync(Guid id);
}
