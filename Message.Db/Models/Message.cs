namespace Message.Db.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime LastModified { get; set; }
    }
}
