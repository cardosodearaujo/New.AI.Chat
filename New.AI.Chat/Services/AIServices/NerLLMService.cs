using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.DTOs;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Extensions;
using New.AI.Chat.Services.Interfaces.AIInterfaces;

namespace New.AI.Chat.Services.AIServices
{
    public class NerLLMService : ILLMStrategyService
    {
        private const string DEFAULT_PROMPT = @"
            Você é um extrator ESTRITO de entidades técnicas.
            Sua única função é encontrar siglas, códigos e nomes de programas na frase do utilizador e devolver um JSON no formato [""termo1"", ""termo2""].

            REGRAS ABSOLUTAS:
            1. Você DEVE extrair a palavra EXATAMENTE como o utilizador digitou.
            2. É ESTRITAMENTE PROIBIDO adicionar extensões de arquivos (ex: .cs, .aspx, .sql).
            3. É ESTRITAMENTE PROIBIDO adicionar caminhos de pastas (ex: DataTables/, Views/).

            Exemplos de comportamento exigido:
            Entrada: ""Gostaria de saber em quais arquivos esses código aparecem: CTB0075 e CTB0007""
            Saída: [""CTB0075"", ""CTB0007""]

            Entrada: ""Como a NFe calcula o ICMS?""
            Saída: [""NFe"", ""ICMS""]

            Entrada: ""Estou recebendo o erro: MAININESP1 Parameter 'PIPI_IDIMPOSTO' not found. O que ele significa e onde é provável que esteja ocorrendo?""
            Saída: [""MAININESP1"", ""PIPI_IDIMPOSTO""]

            Entrada: ""{0}""";

        public LLMEnum LLM => LLMEnum.LightModel;

        public LLMParametersDTO Parameters
        {
            get =>
                new LLMParametersDTO
                {
                    TakeLowGranularitySemanticIDs = 0,
                    TakeLowGranularityWithHighGranularitySemanticIDs = 0,
                    TakeLowGranularityWithHighGranularityLexicalIDs = 0
                };
        }

        private readonly Kernel _kernel;

        public NerLLMService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<string> BuildPromptResponse(string userPrompt , string systemPrompt = "")
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(DEFAULT_PROMPT);
            chatHistory.AddUserMessage(userPrompt);

            var response = await _kernel.GetRequiredService<IChatCompletionService>(LLMEnum.Qwen15.GetDescription())
                                        .GetChatMessageContentAsync(chatHistory);

            return response.Content;
        }
    }
}
