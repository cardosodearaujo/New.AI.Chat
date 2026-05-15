using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class UpdateUserService : DefaultService<(Guid id, UpdateUserDTO dto), bool>, IUpdateUserService
    {
        private readonly AIDbContext _dbContext;

        public UpdateUserService(AIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task Validate((Guid id, UpdateUserDTO dto) entry) => Task.CompletedTask;

        protected override async Task DoProcess((Guid id, UpdateUserDTO dto) payload)
        {
            var (id, dto) = payload;
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                AddError($"Usuário com ID {id} não encontrado.");
                return;
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.DbSetUsers.Update(user);
            await _dbContext.SaveChangesAsync();

            Data = true;
        }
    }
}
