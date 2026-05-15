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

        protected override Task Validate((Guid id, UpdateUserDTO dto) entry, CancellationToken cancellationToken) => Task.CompletedTask;

        private bool _result;

        protected override async Task DoProcess((Guid id, UpdateUserDTO dto) payload, CancellationToken cancellationToken)
        {
            var (id, dto) = payload;
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                AddError($"Usuário com ID {id} não encontrado.");
                return;
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.DbSetUsers.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _result = true;
        }

        protected override Task<bool> GetResultData((Guid id, UpdateUserDTO dto) entry, CancellationToken cancellationToken) => Task.FromResult(_result);
    }
}
