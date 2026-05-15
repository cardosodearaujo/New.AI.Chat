using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class ChangeUserPasswordService : DefaultService<(Guid id, ChangePasswordDTO dto), bool>, IChangeUserPasswordService
    {
        private readonly AIDbContext _dbContext;
        private readonly IPasswordHashService _passwordHashService;

        public ChangeUserPasswordService(AIDbContext dbContext, IPasswordHashService passwordHashService)
        {
            _dbContext = dbContext;
            _passwordHashService = passwordHashService;
        }

        protected override Task Validate((Guid id, ChangePasswordDTO dto) entry, CancellationToken cancellationToken) => Task.CompletedTask;

        private bool _result;

        protected override async Task DoProcess((Guid id, ChangePasswordDTO dto) payload, CancellationToken cancellationToken)
        {
            var (id, dto) = payload;
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                AddError($"Usuário com ID {id} não encontrado.");
                return;
            }

            if (!_passwordHashService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            {
                AddError("Senha atual inválida.");
                return;
            }

            user.PasswordHash = _passwordHashService.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.DbSetUsers.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _result = true;
        }

        protected override Task<bool> GetResultData((Guid id, ChangePasswordDTO dto) entry, CancellationToken cancellationToken) => Task.FromResult(_result);
    }
}
