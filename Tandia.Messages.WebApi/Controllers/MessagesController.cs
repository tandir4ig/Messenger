namespace Tandia.Messages.WebApi.Controllers;

using System;
using Microsoft.AspNetCore.Mvc;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.WebApi.DTOs.Requests;

[ApiController]
[Route("api/Messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService messageService;

    public MessagesController(IMessageService messageService)
    {
        this.messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> Get()
    {
        var messages = await this.messageService.GetAllAsync();
        return this.Ok(messages);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] MessageRequestDto input)
    {
        var message = await this.messageService.SendMessageAsync(id, new Message(input.Content));

        if (message.LastModified is null)
        {
            return this.CreatedAtAction(nameof(this.Get), new { id = message.Id }, message);
        }

        return this.NoContent();
    }
}
