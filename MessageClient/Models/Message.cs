namespace MessageClient.Models;

public class Message
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
