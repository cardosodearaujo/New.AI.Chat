using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Extensions;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;
using Pgvector;
using System.Text;

namespace New.AI.Chat.Services
{
    public class IngestionService : DefaultService<IngestionDTO, IngestionResponseDTO>, IIngestionService
    {
        private string DateNow { get => DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); }

        private readonly ILogger<IngestionService> _logger;
        private readonly Kernel _kernel;
        private readonly AIDbContext _aiDbContext;

        public IngestionService(
            ILogger<IngestionService> logger,
            AIDbContext aiDbContext,
            Kernel kernel)
        {
            _logger = logger;
            _aiDbContext = aiDbContext;
            _kernel = kernel;
        }

        protected override async Task Validate(IngestionDTO ingestionFile)
        {
            if (ingestionFile?.IngestionFiles == null || !ingestionFile.IngestionFiles.Any())
            {
                AddError("A lista de arquivos está vazia ou nula!");
            }
            else
            {
                foreach (var file in ingestionFile.IngestionFiles)
                {
                    if (file is null)
                    {
                        AddError("Lista de arquivos vazia!");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(file.FileName))
                        {
                            AddError("O nome do arquivo é obrigatório.");
                        }

                        if (string.IsNullOrEmpty(file.Format))
                        {
                            AddError($"O formato do arquivo {file.FileName} é obrigatório.");
                        }

                        if (file.Size <= 0)
                        {
                            AddError($"O tamanho do arquivo {file.FileName} é obrigatório.");
                        }

                        if (string.IsNullOrEmpty(file.ContentText))
                        {
                            AddError($"O conteúdo do arquivo {file.FileName} está vazio!");
                        }
                        else if (!file.ContentText.IsBase64String())
                        {
                            AddError($"Formato inválido no arquivo {file.FileName}. O tipo esperado é um base64.");
                        }
                    }
                }
            }
        }

        protected override async Task DoProcess(IngestionDTO ingestionFile)
        {
            int interador = 0;

            var generatorVectors = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

            foreach (var file in ingestionFile.IngestionFiles)
            {
                try
                {
                    interador++;

                    _logger.LogInformation($"{DateNow} - Arquivo {file.FileName}: {interador} de {ingestionFile.IngestionFiles.Count()}");

                    var fileExists = await _aiDbContext.DbSetKDInformation.AnyAsync(f => f.FileName == file.FileName);

                    if (!fileExists)
                    {
                        var originalFileInBytes = Convert.FromBase64String(file.ContentText);

                        var originalFile = Encoding.UTF8.GetString(originalFileInBytes);

                        var kDInformation = new KnowledgeDocumentInformation
                        {
                            FileName = file.FileName,
                            Format = file.Format,
                            Size = file.Size,
                            ContextText = originalFile,
                            DateFirstInsertion = DateTime.UtcNow,
                            DateLastInsertion = DateTime.UtcNow,
                            LowGranularityList = await GetLowGranularity(generatorVectors, originalFile)
                        };

                        await _aiDbContext.DbSetKDInformation.AddAsync(kDInformation);
                        await _aiDbContext.SaveChangesAsync();

                        _logger.LogInformation($"{DateNow} - Arquivo {file.FileName}: Inserido com sucesso.");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateNow} - Arquivo {file.FileName} já existe na base de dados.");
                    }
                }
                catch (Exception ex)
                {
                    AddError($"{DateNow} - Erro ao processar o arquivo {file.FileName}: {ex.Message} | {ex?.InnerException?.Message}");
                    _logger.LogWarning($"{DateNow} - Erro ao processar o arquivo {file.FileName}: {ex.Message} | {ex?.InnerException?.Message}");
                }
            }
        }

        private async Task<List<KnowledgeDocumentLowGranularity>> GetLowGranularity(
            IEmbeddingGenerator<string, Embedding<float>> generatorVectors,
            string originalFile)
        {
            var maxTokensPerLine = 120;
            var maxTokensPerParagraph = 800;
            var overlapTokens = 30;

            var granularityList = new List<KnowledgeDocumentLowGranularity>();

            var lines = TextChunker.SplitPlainTextLines(originalFile, maxTokensPerLine: maxTokensPerLine);

            var chunks = TextChunker.SplitPlainTextParagraphs(lines, maxTokensPerParagraph: maxTokensPerParagraph, overlapTokens: overlapTokens);

            var vectors = await generatorVectors.GenerateAsync(chunks);

            var vectorsAndChunks = chunks.Zip(vectors.Select(F => F.Vector).ToList());

            foreach (var (chunk, vector) in vectorsAndChunks)
            {
                var granularity = new KnowledgeDocumentLowGranularity
                {
                    ContentText = chunk,
                    Embedding = new Vector(vector.ToArray()),
                    HighGranularityList = await GetHighGranularity(generatorVectors, chunk)
                };

                granularityList.Add(granularity);
            }

            return granularityList;
        }

        private async Task<List<KnowledgeDocumentHighGranularity>> GetHighGranularity(
            IEmbeddingGenerator<string, Embedding<float>> generatorVectors, 
            string originalFile)
        {
            var maxTokensPerLine = 60;
            var maxTokensPerParagraph = 150;
            var overlapTokens = 30;

            var granularityList = new List<KnowledgeDocumentHighGranularity>();

            var lines = TextChunker.SplitPlainTextLines(originalFile, maxTokensPerLine: maxTokensPerLine);

            var chunks = TextChunker.SplitPlainTextParagraphs(lines, maxTokensPerParagraph: maxTokensPerParagraph, overlapTokens: overlapTokens);

            var vectors = await generatorVectors.GenerateAsync(chunks);

            var vectorsAndChunks = chunks.Zip(vectors.Select(F => F.Vector).ToList());

            foreach (var (chunk, vector) in vectorsAndChunks)
            {
                var granularity = new KnowledgeDocumentHighGranularity
                {
                    ContentText = chunk,
                    Embedding = new Vector(vector.ToArray())
                };

                granularityList.Add(granularity);
            }

            return granularityList;
        }        
    }
}
