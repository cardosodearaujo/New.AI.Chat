using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class DeleteUserService : DefaultService<Guid, bool>, IDeleteUserService
    {
        private readonly AIDbContext _dbContext;

        public DeleteUserService(AIDbContext dbContext)
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

            _dbContext.DbSetUsers.Remove(user);
            await _dbContext.SaveChangesAsync();

            Data = true;
        }
    }
}
