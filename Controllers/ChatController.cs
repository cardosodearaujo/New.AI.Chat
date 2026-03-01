using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : DefaultController<PromptDTO, ResponseDTO>
    {
        private readonly IChatService _chatService;

        public ChatController(
            ILogger<ChatController> logger,
            IChatService chatService) : base(logger)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<Results<Ok<ResponseDTO>, BadRequest<IList<string>>>> SendMessage(PromptDTO message)
        {
            return await Process(_chatService, message);
        }
    }
}