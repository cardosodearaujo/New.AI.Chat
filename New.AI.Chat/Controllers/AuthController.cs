using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : DefaultController
    {
        private readonly IAuthService _authService;

        public AuthController(
            ILogger<DefaultController> logger, 
            IAuthService authService) : base(logger)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            await _authService.Process(login);

            var r = _authService.Result;
            if (r == null || !r.IsSuccess)
                return Unauthorized(r?.Errors);

            return Ok(r.Data);
        }
    }
}
