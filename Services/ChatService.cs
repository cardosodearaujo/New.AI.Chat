using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Services.Interfaces;
using Pgvector.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace New.AI.Chat.Services
{
    public class ChatService : DefaultService<PromptDTO, PromptResponseDTO>, IChatService
    {
        private readonly AIDbContext _aiDbContext;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _vectorGenerator;
        private readonly ILLMStrategyFactoryService _LLMStrategyFactory;
        private readonly ILogger<ChatService> _logger;

        public string DateTimeNow => DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");

        public ChatService(
            Kernel kernel,
            AIDbContext aiDbContext,
            ILogger<ChatService> logger,
            ILLMStrategyFactoryService LLMStrategyFactory) : base()
        {
            _aiDbContext = aiDbContext;
            _logger = logger;
            _LLMStrategyFactory = LLMStrategyFactory;
            _vectorGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        }

        protected override async Task Validate(PromptDTO prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt?.Message))
            {
                AddError("A mensagem do utilizador não pode ser vazia!");
            }

            if ((prompt.LLM is null) || (!prompt.LLM.HasValue))
            {
                AddError("É obrigatório informar o LLM.");
            }
            else
            {
                if (!Enum.IsDefined(typeof(LLMEnum), prompt.LLM.Value))
                {
                    AddError("LLM informado não é válido.");
                }
            }
        }

        protected override async Task DoProcess(PromptDTO prompt)
        {
            //Passo1: Incorpora o prompt:
            var promptEmbedding = await _vectorGenerator.GenerateAsync([prompt.Message]);

            if (promptEmbedding.Any())
            {
                //Passo 2: Gera o vetor do prompt:
                var promptVector = new Pgvector.Vector(promptEmbedding?.FirstOrDefault()?.Vector.ToArray());

                var lowGranularityIDs = new HashSet<Guid>();

                //Passo 3: Busca dados de baixa granularidade com base no prompt enviado pelo usuário:
                lowGranularityIDs.UnionWith(await GetLowGranularitySemanticIDs(promptVector));

                //Passo 4: Busca dados de baixa granulariade a partir de dados de alta granularidade com base no prompt enviado pelo usuário:
                lowGranularityIDs.UnionWith(await GetLowGranularityWithHighGranularitySemanticIDs(promptVector));

                //Passo 3: Busca os dados de baixa granulariade com base em dados de alta granulariade usando termos especificos extraidos
                //por IA que estejam no prompt enviado pelo usuário:
                lowGranularityIDs.UnionWith(await GetLowGranularityWithHighGranularityLexicalIDs(prompt.Message));

                //Passo 4: Busca todas as instancias de baixa granularidade selecionadas nos passos anteriores:
                var referenceData = await _aiDbContext.DbSetKDLowGranularity
                                                      .AsNoTracking()
                                                      .Include(p => p.KnowledgeDocumentInformation)
                                                      .Where(p => lowGranularityIDs.Contains(p.Id))
                                                      .ToListAsync();

                //Passo 5: Caso tenha dados constroi o contexto do prompt:
                if (referenceData.Any())
                {
                    var systemPrompt = new StringBuilder();
                    var referenceFiles = new List<string>();

                    foreach (var reference in referenceData)
                    {
                        var fileName = reference?.KnowledgeDocumentInformation?.FileName;

                        referenceFiles.Add(fileName);

                        systemPrompt.AppendLine($"--- Arquivo: {fileName} ---");
                        systemPrompt.AppendLine(reference.ContentText);
                    }

                    //Passo 6: Chama a estatégia para resolver qual LLM irá responder a pergunta:
                    var response = await _LLMStrategyFactory.GetStrategy(prompt.LLM.Value)
                                                            .BuildPromptResponse(prompt.Message,
                                                                                 systemPrompt.ToString());

                    //Passo 7: Envia a resposta para o usuário:
                    if (response != null)
                    {
                        Data = new PromptResponseDTO
                        {
                            Response = response,
                            ReferenceFiles = referenceFiles.ToList(),
                            DateTime = DateTimeNow
                        };
                    }
                    else
                    {
                        AddError($"O modelo {prompt.LLM} não retornou nenhuma resposta válida.");
                    }
                }
                else
                {
                    AddError("Nenhum contexto relevante foi encontrado na base de código.");
                }
            }
        }

        private async Task<List<Guid>> GetLowGranularitySemanticIDs(Pgvector.Vector promptVector)
        {
            return await _aiDbContext.DbSetKDLowGranularity
                                     .OrderBy(p => p.Embedding.L2Distance(promptVector))
                                     .Take(2)
                                     .Select(p => p.Id)
                                     .ToListAsync();
        }

        private async Task<List<Guid>> GetLowGranularityWithHighGranularitySemanticIDs(Pgvector.Vector promptVector)
        {
            return await _aiDbContext.DbSetKDHighGranularity
                                     .OrderBy(c => c.Embedding.L2Distance(promptVector))
                                     .Take(3)
                                     .Select(c => c.LowGranularityId)
                                     .ToListAsync();
        }

        private async Task<List<Guid>> GetLowGranularityWithHighGranularityLexicalIDs(string message)
        {
            var technicalTerms = await ExtractTechnicalTerms(message);
            var allHighGranularityLexicalIDs = new List<Guid>();

            if (technicalTerms.Any())
            {
                foreach (var terms in technicalTerms)
                {
                    var highGranularityLexicalIDs = await _aiDbContext.DbSetKDHighGranularity
                                                                      .Where(c => EF.Functions.ILike(c.ContentText, $"%{terms}%"))
                                                                      .Take(2)
                                                                      .Select(c => c.LowGranularityId)
                                                                      .ToListAsync();

                    allHighGranularityLexicalIDs.AddRange(highGranularityLexicalIDs);
                }
            }

            return allHighGranularityLexicalIDs;
        }

        private async Task<List<string>> ExtractTechnicalTerms(string message)
        {
            var terms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var response = await _LLMStrategyFactory.GetStrategy(LLMEnum.LightModel)
                                                    .BuildPromptResponse(message);

            if (!string.IsNullOrWhiteSpace(response))
            {
                try
                {
                    var cleanResponse = response.Replace("```json", "").Replace("```", "").Trim();
                    var extractTerms = JsonSerializer.Deserialize<List<string>>(cleanResponse,
                                                                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (extractTerms != null)
                    {
                        foreach (var T in extractTerms.Where(X => !string.IsNullOrWhiteSpace(X))) terms.Add(T.Trim());
                        _logger.LogInformation("Termos extraídos pela IA: {Termos}", string.Join(", ", terms));
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "A IA retornou um formato inválido ao extrair termos. Resposta crua: {Response}", response);
                }
            }

            return terms.ToList();
        }
    }
}