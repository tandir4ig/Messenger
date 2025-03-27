using Microsoft.AspNetCore.Mvc;
using Tandia.Messages.Application.Enums;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.WebApi.DTOs.Requests;

namespace Tandia.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMessageService messageService;

    public MessagesController(IMessageService messageService)
    {
        this.messageService = messageService;
    }

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

        if (result is MessageStatus.Created)
        {
            return StatusCode(201);
        }

        if (result is MessageStatus.Updated)
        {
            return StatusCode(204);
        }

        return StatusCode(400);
    }
}
