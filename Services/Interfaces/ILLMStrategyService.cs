using Microsoft.SemanticKernel;
using New.AI.Chat.Enumerators;

namespace New.AI.Chat.Services.Interfaces
{
    public interface ILLMStrategyService
    {
        public LLMEnum LLM { get; }

        Task<string> BuildPromptResponse(string userPrompt, string systemPrompt = "");
    }
}
