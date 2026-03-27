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
            Data = false;
        }

        protected override async Task Validate(FileQueryDTO entry)
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

        protected override async Task DoProcess(FileQueryDTO entry)
        {
            Data = await _aiDbContext.DbSetKDInformation.AnyAsync(f => f.FileName == entry.FileName);
        }
    }
}
