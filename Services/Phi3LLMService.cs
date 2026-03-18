using Microsoft.OpenApi;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class Phi3LLMService : ILLMStrategyService
    {
        private const string DEFAULT_PROMPT = @"
            Você é um Arquiteto de Software Sênior e Especialista de Sistemas focado em ERP.
            Responda detalhadamente utilizando APENAS o contexto fornecido.
            Se a resposta exigir código, apresente-o. Se não estiver no contexto, diga que não encontrou.
            CONTEXTO ARQUITETURAL:
            {0}";

        public LLMEnum LLM => LLMEnum.Phi3;

        private readonly Kernel _kernel;

        public Phi3LLMService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<string> BuildPromptResponse(string systemPrompt, string userPrompt)
        {
            var finalSystemPrompt = string.Format(DEFAULT_PROMPT, systemPrompt);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(finalSystemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await _kernel.GetRequiredService<IChatCompletionService>(LLMEnum.Phi3.GetDisplayName())
                                        .GetChatMessageContentAsync(chatHistory);

            return response.Content;
        }
    }
}
