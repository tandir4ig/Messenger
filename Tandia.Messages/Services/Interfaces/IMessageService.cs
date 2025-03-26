using Tandia.Messages.Application.Models;

namespace Tandia.Messages.Application.Services.Interfaces;

public interface IMessageService
{
    Task<IReadOnlyCollection<Message?>> GetAllAsync();

    Task<Message> SendMessageAsync(Message message);
}
