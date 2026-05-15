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

        protected override Task Validate(object entry, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private GetAuthenticationLogsResponseDTO? _result;

        protected override async Task DoProcess(object entry, CancellationToken cancellationToken)
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
                .ToListAsync(cancellationToken);

            _result = new GetAuthenticationLogsResponseDTO
            {
                Logs = logs,
                TotalRecords = logs.Count
            };
        }

        protected override Task<GetAuthenticationLogsResponseDTO> GetResultData(object entry, CancellationToken cancellationToken) => Task.FromResult(_result!);
    }
}
