using New.AI.Chat.Enumerators;

namespace New.AI.Chat.Services.Interfaces.AIInterfaces
{
    public interface ILLMStrategyFactoryService
    {
        ILLMStrategyService GetStrategy(LLMEnum LLM);
    }
}
