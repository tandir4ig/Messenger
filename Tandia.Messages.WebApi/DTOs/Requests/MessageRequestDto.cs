namespace Tandia.Messages.WebApi.DTOs.Requests;

public record MessageRequestDto
{
    required public string Content { get; set; }
}
