﻿using Tandia.Messages.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Tandia.Messages.Models;
using Tandia.Messages.WebApi.DTOs.Requests;

namespace Tandia.Messages.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : Controller
{
    private readonly IMessageRepository _messageRepository;

    public MessagesController(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string content)
    {
        var message = new Message
        {
            Content = content,
            Timestamp = DateTime.UtcNow,
        };

        await _messageRepository.AddAsync(message);

        return CreatedAtAction(nameof(Get), new { id = message.Id }, message);
    }

    //[HttpPost]
    //public async Task<IActionResult> Post([FromBody] MessageRequestDto input)
    //{
    //    var _message = new Message
    //    {
    //        Content = input.Content,
    //        Timestamp = DateTime.UtcNow,
    //    };

    //    await _messageRepository.AddAsync(_message);

    //    return CreatedAtAction(nameof(Get), new { id = _message.Id }, _message);
    //}

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> Get()
    {
        var messages = await _messageRepository.GetAllAsync();
        return Ok(messages);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] MessageRequestDto input)
    {
        var message = await _messageRepository.GetMessageByid(id);

        if (message is null)
        {
            message = new Message
            {
                Content = input.Content,
                Timestamp = DateTime.UtcNow,
            };

            await _messageRepository.AddAsync(message);

            return CreatedAtAction(nameof(Get), new { id = message.Id }, message);
        }
        else
        {
            message.Content = input.Content;
            message.LastModified = DateTime.UtcNow;

            await _messageRepository.UpdateMessageAsync(message);

            return NoContent();
        }
    }
}
