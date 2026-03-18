using New.AI.Chat.Enumerators;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class LLMStrategyFactoryService : ILLMStrategyFactoryService
    {

        private readonly IList<ILLMStrategyService> _LLMStrategyServices;

        public LLMStrategyFactoryService(IList<ILLMStrategyService> LLMStrategyServices)
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
