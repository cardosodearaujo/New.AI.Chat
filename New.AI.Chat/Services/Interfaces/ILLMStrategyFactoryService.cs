using New.AI.Chat.Enumerators;

namespace New.AI.Chat.Services.Interfaces
{
    public interface ILLMStrategyFactoryService
    {
        ILLMStrategyService GetStrategy(LLMEnum LLM);
    }
}
