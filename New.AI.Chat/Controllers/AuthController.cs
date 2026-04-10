using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : DefaultController<LoginDTO, AuthResponseDTO>
    {
        private readonly IAuthService _authService;

        public AuthController(
            ILogger<DefaultController<LoginDTO, AuthResponseDTO>> logger, 
            IAuthService authService) : base(logger)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            await _authService.Process(login);

            if (_authService.HasErrors())
                return Unauthorized(_authService.Messages);

            return Ok(_authService.Data);
        }
    }
}
