using New.AI.Chat.Enumerators;
using New.AI.Chat.Services.Interfaces.AIInterfaces;

namespace New.AI.Chat.Services.AIServices
{
    public class LLMStrategyFactoryService : ILLMStrategyFactoryService
    {

        private readonly IEnumerable<ILLMStrategyService> _LLMStrategyServices;

        public LLMStrategyFactoryService(IEnumerable<ILLMStrategyService> LLMStrategyServices)
        {
            _LLMStrategyServices = LLMStrategyServices;
        }

        public ILLMStrategyService GetStrategy(LLMEnum LLM)
        {
            var LLMSelected = _LLMStrategyServices.FirstOrDefault(s => s.LLM == LLM);

            if (LLMSelected == null)
            {
                throw new NotSupportedException($"O modelo {LLM} ainda não possui uma estratégia implementada.");
            }

            return LLMSelected;
        }
    }
}
