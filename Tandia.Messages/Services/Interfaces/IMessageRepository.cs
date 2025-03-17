namespace Tandia.Messages.Services.Interfaces;

using Tandia.Messages.Data.Entities;

public interface IMessageRepository
{
    Task<IEnumerable<Message?>> GetAllAsync();

    Task AddAsync (Message message);

    Task UpdateMessageAsync (Message message);

    Task<Message?> GetMessageByid(Guid id);
}
