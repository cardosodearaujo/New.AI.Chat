using New.AI.Chat.DTOs;
using New.AI.Chat.Enumerators;

namespace New.AI.Chat.Services.Interfaces
{
    public interface ILLMStrategyService
    {
        public LLMEnum LLM { get; }

        public LLMParametersDTO Parameters { get; }

        Task<string> BuildPromptResponse(string userPrompt, string systemPrompt = "");
    }
}
