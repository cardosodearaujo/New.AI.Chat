using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using New.AI.Chat.DTOs;
using New.AI.Chat.Enumerators;
using New.AI.Chat.Extensions;
using New.AI.Chat.Services.Interfaces.AIInterfaces;

namespace New.AI.Chat.Services.AIServices
{
    public class Qwen15LLMService : ILLMStrategyService
    {
        private const string DEFAULT_PROMPT = @"
            Aja como um desenvolvedor C# ou delphi. Leia o contexto abaixo e responda à pergunta.
            Regra 1: Use apenas o código do contexto.
            Regra 2: Seja direto e curto.
            Regra 3: Se não achar a resposta, diga 'Não encontrado'.
            CONTEXTO:
            {0}";

        public LLMEnum LLM => LLMEnum.Qwen15;

        public LLMParametersDTO Parameters 
        { 
            get => 
                new LLMParametersDTO 
                {
                    TakeLowGranularitySemanticIDs = 2,
                    TakeLowGranularityWithHighGranularitySemanticIDs = 3,
                    TakeLowGranularityWithHighGranularityLexicalIDs = 2
                }; 
        }

        private readonly Kernel _kernel;

        public Qwen15LLMService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<string> BuildPromptResponse(string systemPrompt, string userPrompt)
        {
            var finalSystemPrompt = string.Format(DEFAULT_PROMPT, systemPrompt);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(finalSystemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await _kernel.GetRequiredService<IChatCompletionService>(LLMEnum.Qwen15.GetDescription())
                                        .GetChatMessageContentAsync(chatHistory);

            return response.Content;
        }
    }
}
