using Message.Db.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMessage.Controllers
{
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
            var message = new Message.Db.Models.Message
            {
                Content = content,
                Timestamp = DateTime.UtcNow,
            };

            await _messageRepository.AddAsync(message);

            return CreatedAtAction(nameof(Get), new { id = message.Id }, message);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message.Db.Models.Message>>> Get()
        {
            var messages = await _messageRepository.GetAllAsync();
            return Ok(messages);
        }
    }
}
