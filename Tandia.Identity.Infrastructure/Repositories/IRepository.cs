namespace Tandia.Identity.Infrastructure.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);

    Task<T?> GetByEmailAsync(string email);

    Task<IReadOnlyCollection<T>> GetAllAsync();

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(Guid id);
}
