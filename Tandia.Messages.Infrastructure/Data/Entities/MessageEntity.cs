namespace Tandia.Messages.Infrastructure.Data.Entities;

public class MessageEntity
{
    public MessageEntity()
    {
    }

    public MessageEntity(string content)
    {
        Content = content;
    }

    public Guid Id { get; set; }

    public string Content { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
