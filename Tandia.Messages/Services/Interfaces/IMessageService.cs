namespace Tandia.Messages.Application.Services.Interfaces;

using Tandia.Messages.Application.Models;

public interface IMessageService
{
    Task<IEnumerable<Message?>> GetAllAsync();

    Task<Message> SendMessageAsync(Guid id, Message message);

    Task<Message?> GetMessageByidAsync(Guid id);
}
