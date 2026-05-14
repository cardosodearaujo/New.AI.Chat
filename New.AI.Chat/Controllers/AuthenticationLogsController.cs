using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationLogsController : DefaultController<object, GetAuthenticationLogsResponseDTO>
    {
        private readonly IGetAuthenticationLogsService _getAuthenticationLogsService;

        public AuthenticationLogsController(
            ILogger<DefaultController<object, GetAuthenticationLogsResponseDTO>> logger,
            IGetAuthenticationLogsService getAuthenticationLogsService) : base(logger)
        {
            _getAuthenticationLogsService = getAuthenticationLogsService;
        }

        [HttpGet]
        public async Task<Results<Ok<GetAuthenticationLogsResponseDTO>, BadRequest<IList<string>>>> GetAll()
        {
            return await Process(_getAuthenticationLogsService, null);
        }
    }
}
