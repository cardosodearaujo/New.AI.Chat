using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Extensions;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;
using Pgvector;

namespace New.AI.Chat.Services
{
    public class IngestionService : DefaultService<IngestionDTO, IngestionResponseDTO>, IIngestionService
    {
        private readonly ILogger<IngestionService> _logger;
        private readonly Kernel _kernel;
        private readonly AIDbContext _aiDbContext;

        public IngestionService(ILogger<IngestionService> logger,
            AIDbContext aiDbContext, 
            Kernel kernel)
        {
            _logger = logger;
            _aiDbContext = aiDbContext;
            _kernel = kernel;
        }

        protected override async Task Validate(IngestionDTO ingestionFile)
        {
            if (ingestionFile is null)
            {
                AddError("Arquivo vazio!");
            }
            else
            {
                foreach (var file in ingestionFile.IngestionFiles)
                {
                    if (file is null)
                    {
                        AddError("Arquivo vazio!");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(file.FileName))
                        {
                            AddError("O nome do arquivo é obrigatório.");
                        }

                        if (string.IsNullOrEmpty(file.Content))
                        {
                            AddError("Arquivo vazio!");
                        }
                        else if (!file.Content.IsBase64String())
                        {
                            AddError("Forma inválido. O tipo esperado é um base64.");
                        }
                    }
                } 
            }
        }

        protected override async Task DoProcess(IngestionDTO ingestionFile)
        {
            var interador = 0;
            var doSave = false;

            foreach (var file in ingestionFile.IngestionFiles)
            {
                interador++;

                _logger.Log(LogLevel.Warning, $"{DateTime.Now.ToString("dd/MM/yyyy")} - Arquivo {file.FileName}: {interador} de {ingestionFile.IngestionFiles.Count()}");

                var exists = _aiDbContext.KnowledgeDocumentDbSet.Any(F => F.Font == file.FileName);             

                if (!exists)
                {
                    doSave = true;

                    var originalFileInBytes = Convert.FromBase64String(file.Content);

                    var originalFile = System.Text.Encoding.UTF8.GetString(originalFileInBytes);

                    var lines = TextChunker.SplitPlainTextLines(originalFile, maxTokensPerLine: 40);

                    var chunkers = TextChunker.SplitPlainTextParagraphs(
                        lines,
                        maxTokensPerParagraph: 50,
                        overlapTokens: 15
                    );

                    var generatorVectors = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

                    var vectors = await generatorVectors.GenerateAsync(chunkers);

                    if (chunkers.Count != vectors.Count)
                    {
                        AddError("Inconsistência na IA: O número de vetores gerados difere do número de fatias.");
                    }

                    var chunkersAndVectors = chunkers.Zip(vectors);

                    foreach (var (chunkerText, chunkerVector) in chunkersAndVectors)
                    {
                        var knowledgeDocument = new KnowledgeDocument
                        {
                            Font = file.FileName,
                            ContentText = chunkerText,
                            Embedding = new Vector(chunkerVector.Vector.ToArray())
                        };

                        _aiDbContext.KnowledgeDocumentDbSet.Add(knowledgeDocument);
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Warning, $"{DateTime.Now.ToString("dd/MM/yyyy")} - Arquivo {file.FileName} existe na base de dados: {exists}");
                }

                if (doSave) await _aiDbContext.SaveChangesAsync();
            }
        }
    }
}
