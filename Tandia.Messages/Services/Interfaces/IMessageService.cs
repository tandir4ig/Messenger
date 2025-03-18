namespace Tandia.Messages.Services.Interfaces;

using Tandia.Messages.Data.Entities;

public interface IMessageService
{
    Task<IEnumerable<Message?>> GetAllAsync();

    Task<Message> SendMessageAsync(Guid id, Message message);

    Task UpdateMessageAsync(Message message);

    Task<Message?> GetMessageByidAsync(Guid id);
}
