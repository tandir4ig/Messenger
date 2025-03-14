using Tandia.Messages.Models;

namespace Tandia.Messages.Services.Interfaces;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetAllAsync();
    Task AddAsync (Message message);
    Task UpdateMessageAsync (Message message);
    Task<Message> GetMessageByid(Guid id);
}
