namespace Message.Db.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Context { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
