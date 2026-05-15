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

        protected override Task Validate(Guid entry) => Task.CompletedTask;

        protected override async Task DoProcess(Guid id)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                AddError($"Usuário com ID {id} não encontrado.");
                return;
            }

            Data = new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
