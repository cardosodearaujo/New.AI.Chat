using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class GetAuthenticationLogsService : DefaultService<object, GetAuthenticationLogsResponseDTO>, IGetAuthenticationLogsService
    {
        private readonly AIDbContext _dbContext;

        public GetAuthenticationLogsService(AIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task Validate(object entry)
        {
            return Task.CompletedTask;
        }

        protected override async Task DoProcess(object entry)
        {
            var logs = await _dbContext.DbSetAuthenticationLogs
                .AsNoTracking()
                .OrderByDescending(l => l.LoginDateTime)
                .Select(l => new AuthenticationLogResponseDTO
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Username = l.Username,
                    LoginDateTime = l.LoginDateTime,
                    TokenExpiresAt = l.TokenExpiresAt,
                    IsSuccessful = !string.IsNullOrEmpty(l.Token)
                })
                .ToListAsync();

            Data = new GetAuthenticationLogsResponseDTO
            {
                Logs = logs,
                TotalRecords = logs.Count
            };
        }
    }
}
