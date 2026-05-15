using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class GetUsersService : DefaultService<object, GetUsersResponseDTO>, IGetUsersService
    {
        private readonly AIDbContext _dbContext;

        public GetUsersService(AIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task Validate(object entry, CancellationToken cancellationToken) => Task.CompletedTask;

        private GetUsersResponseDTO? _result;

        protected override async Task DoProcess(object entry, CancellationToken cancellationToken)
        {
            var users = await _dbContext.DbSetUsers
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Username = u.Username,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync(cancellationToken);
            _result = new GetUsersResponseDTO { Users = users };
        }

        protected override Task<GetUsersResponseDTO> GetResultData(object entry, CancellationToken cancellationToken) => Task.FromResult(_result!);
    }
}
