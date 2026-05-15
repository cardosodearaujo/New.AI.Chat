using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : DefaultController
    {
        private readonly IGetUsersService _getUsersService;
        private readonly IGetUserByIdService _getUserByIdService;
        private readonly ICreateUserService _createUserService;
        private readonly IUpdateUserService _updateUserService;
        private readonly IDeleteUserService _deleteUserService;
        private readonly IChangeUserPasswordService _changeUserPasswordService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            ILogger<DefaultController> logger,
            IGetUsersService getUsersService,
            IGetUserByIdService getUserByIdService,
            ICreateUserService createUserService,
            IUpdateUserService updateUserService,
            IDeleteUserService deleteUserService,
            IChangeUserPasswordService changeUserPasswordService) : base(logger)
        {
            _logger = logger as ILogger<UsersController> ?? throw new ArgumentNullException(nameof(logger));
            _getUsersService = getUsersService;
            _getUserByIdService = getUserByIdService;
            _createUserService = createUserService;
            _updateUserService = updateUserService;
            _deleteUserService = deleteUserService;
            _changeUserPasswordService = changeUserPasswordService;
        }

        [HttpGet]
        public async Task<Results<Ok<GetUsersResponseDTO>, BadRequest<IList<string>>>> GetAll()
        {
            return await Process(_getUsersService, null);
        }

        [HttpGet("{id}")]
        public async Task<Results<Ok<UserResponseDTO>, BadRequest<IList<string>>>> Get(Guid id)
        {
            return await Process(_getUserByIdService, id);
        }

        [HttpPost]
        public async Task<Results<Ok<UserResponseDTO>, BadRequest<IList<string>>>> Create([FromBody] CreateUserDTO dto)
        {
            return await Process(_createUserService, dto);
        }

        [HttpPut("{id}")]
        public async Task<Results<Ok<bool>, BadRequest<IList<string>>>> Update(Guid id, [FromBody] UpdateUserDTO dto)
        {
            return await Process(_updateUserService, (id, dto));
        }

        [HttpDelete("{id}")]
        public async Task<Results<Ok<bool>, BadRequest<IList<string>>>> Delete(Guid id)
        {
            return await Process(_deleteUserService, id);
        }

        [HttpPost("{id}/change-password")]
        public async Task<Results<Ok<bool>, BadRequest<IList<string>>>> ChangePassword(Guid id, [FromBody] ChangePasswordDTO dto)
        {
            return await Process(_changeUserPasswordService, (id, dto));
        }
    }
}
