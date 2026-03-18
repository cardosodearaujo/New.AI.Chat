using Microsoft.OpenApi;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class NerLLMService : ILLMStrategyService
    {
        private const string DEFAULT_PROMPT = @"
            Você é um extrator de termos técnicos de código-fonte em C#, Delphi, Xml, HTML, TypeScript além regras de negócio de ERP.
            Sua saída deve ser APENAS um JSON válido no formato: { [""string"", ""string""] }
            Não forneça explicações, não crie código, apenas o JSON.
            Ignore palavras comuns da língua portuguesa.
            Foque em: siglas, nomes de variáveis (CamelCase, snake_case), nomes de tabelas, nomes de métodos ou termos técnicos (ex: DIFAL, SQL, API).";

        public LLMEnum LLM => LLMEnum.LightModel;

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

            var response = await _kernel.GetRequiredService<IChatCompletionService>(LLMEnum.LightModel.GetDisplayName())
                                        .GetChatMessageContentAsync(chatHistory);

            return response.Content;
        }
    }
}
