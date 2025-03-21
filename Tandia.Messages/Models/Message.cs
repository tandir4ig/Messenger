namespace Tandia.Messages.Application.Models;

public class Message
{
    public Message()
    {
    }

    public Message(string content)
    {
        Content = content;
    }

    public Guid Id { get; set; }

    public string Content { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
