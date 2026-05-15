using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class FileService : DefaultService<FileQueryDTO, bool>, IFileService
    {
        private readonly AIDbContext _aiDbContext;

        public FileService(AIDbContext aiDbContext)
        {
            _aiDbContext = aiDbContext;
        }

        protected override async Task Validate(FileQueryDTO entry, CancellationToken cancellationToken)
        {
            if (entry == null)
            {
                AddError("Arquivo inválido.");
                return;
            }

            if (string.IsNullOrEmpty(entry.FileName))
            {
                AddError("O nome do arquivo é obrigatório.");
            }
        }

        private bool _result;

        protected override async Task DoProcess(FileQueryDTO entry, CancellationToken cancellationToken)
        {
            _result = await _aiDbContext.DbSetKDInformation.AnyAsync(f => f.FileName == entry.FileName, cancellationToken);
        }

        protected override Task<bool> GetResultData(FileQueryDTO entry, CancellationToken cancellationToken) => Task.FromResult(_result);
    }
}
