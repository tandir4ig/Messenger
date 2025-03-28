using Microsoft.AspNetCore.Mvc;
using Tandia.Messages.Application.Enums;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.WebApi.DTOs.Requests;

namespace Tandia.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Message>>> Get()
    {
        var messages = await messageService.GetAllAsync();
        return Ok(messages);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] MessageRequestDto input)
    {
        var result = await messageService.SendMessageAsync(id, input.Content);

        return result switch
        {
            MessageStatus.Created => Created(),

            MessageStatus.Updated => NoContent(),

            _ => BadRequest(),
        };
    }
}
