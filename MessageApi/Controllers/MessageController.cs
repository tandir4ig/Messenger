using Message.Db.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiMessage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
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

            await _messageRepository.Add(message);

            return CreatedAtAction(nameof(Get), new { id = message.Id }, message);
        }

        [HttpGet]
        public async Task<IEnumerable<Message.Db.Models.Message>> Get()
        {
            return await _messageRepository.GetAll();
        }
    }
}
