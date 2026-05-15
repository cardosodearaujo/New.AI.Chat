using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationLogsController : DefaultController
    {
        private readonly IGetAuthenticationLogsService _getAuthenticationLogsService;

        public AuthenticationLogsController(
            ILogger<DefaultController> logger,
            IGetAuthenticationLogsService getAuthenticationLogsService) : base(logger)
        {
            _getAuthenticationLogsService = getAuthenticationLogsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await Process(_getAuthenticationLogsService, null);
        }
    }
}
