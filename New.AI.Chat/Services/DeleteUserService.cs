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

        protected override Task Validate(Guid entry, CancellationToken cancellationToken) => Task.CompletedTask;

        private bool _result;

        protected override async Task DoProcess(Guid id, CancellationToken cancellationToken)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                AddError($"Usuário com ID {id} não encontrado.");
                return;
            }

            _dbContext.DbSetUsers.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _result = true;
        }

        protected override Task<bool> GetResultData(Guid entry, CancellationToken cancellationToken) => Task.FromResult(_result);
    }
}
