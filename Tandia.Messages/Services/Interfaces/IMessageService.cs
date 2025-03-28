using Tandia.Messages.Application.Enums;
using Tandia.Messages.Application.Models;

namespace Tandia.Messages.Application.Services.Interfaces;

public interface IMessageService
{
    Task<IReadOnlyCollection<Message>> GetAllAsync();

    Task<MessageStatus> SendMessageAsync(Guid id, string content);
}
