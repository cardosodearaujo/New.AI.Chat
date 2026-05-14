using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using New.AI.Chat.DTOs;


namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IGetUsersService _getUsersService;
        private readonly IGetUserByIdService _getUserByIdService;
        private readonly ICreateUserService _createUserService;
        private readonly IUpdateUserService _updateUserService;
        private readonly IDeleteUserService _deleteUserService;
        private readonly IChangeUserPasswordService _changeUserPasswordService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            ILogger<UsersController> logger,
            IGetUsersService getUsersService,
            IGetUserByIdService getUserByIdService,
            ICreateUserService createUserService,
            IUpdateUserService updateUserService,
            IDeleteUserService deleteUserService,
            IChangeUserPasswordService changeUserPasswordService)
        {
            _logger = logger;
            _getUsersService = getUsersService;
            _getUserByIdService = getUserByIdService;
            _createUserService = createUserService;
            _updateUserService = updateUserService;
            _deleteUserService = deleteUserService;
            _changeUserPasswordService = changeUserPasswordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await _getUsersService.Process(null);
            if (_getUsersService.HasErrors()) return BadRequest(_getUsersService.Messages);
            return Ok(_getUsersService.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await _getUserByIdService.Process(id);
            if (_getUserByIdService.HasErrors()) return NotFound();
            return Ok(_getUserByIdService.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            await _createUserService.Process(dto);
            if (_createUserService.HasErrors()) return BadRequest(_createUserService.Messages);
            return CreatedAtAction(nameof(Get), new { id = _createUserService.Data.Id }, _createUserService.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO dto)
        {
            await _updateUserService.Process((id, dto));
            if (_updateUserService.HasErrors()) return BadRequest(_updateUserService.Messages);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _deleteUserService.Process(id);
            if (_deleteUserService.HasErrors()) return BadRequest(_deleteUserService.Messages);
            return NoContent();
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO dto)
        {
            await _changeUserPasswordService.Process((id, dto));
            if (_changeUserPasswordService.HasErrors()) return BadRequest(_changeUserPasswordService.Messages);
            return NoContent();
        }
    }
}
