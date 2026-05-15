using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class GetUserByIdService : DefaultService<Guid, UserResponseDTO>, IGetUserByIdService
    {
        private readonly AIDbContext _dbContext;

        public GetUserByIdService(AIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task Validate(Guid entry, CancellationToken cancellationToken) => Task.CompletedTask;

        private UserResponseDTO? _result;

        protected override async Task DoProcess(Guid id, CancellationToken cancellationToken)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                AddError($"Usuário com ID {id} não encontrado.");
                return;
            }

            _result = new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        protected override Task<UserResponseDTO?> GetResultData(Guid entry, CancellationToken cancellationToken) => Task.FromResult(_result);
    }
}
