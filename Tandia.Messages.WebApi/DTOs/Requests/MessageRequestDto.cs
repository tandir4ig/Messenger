namespace Tandia.Messages.WebApi.DTOs.Requests
{
    public record MessageRequestDto
    {
        public string Content { get; set; }

        public MessageRequestDto(string content)
        {
            Content = content;
        }
    }
}
