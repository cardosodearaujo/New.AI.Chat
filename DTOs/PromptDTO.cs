using New.AI.Chat.Enumerators;

namespace New.AI.Chat.DTOs
{
    public class PromptDTO
    {
        public PromptDTO()
        {
            Message = string.Empty;
            LLM = LLMEnum.Qwen;
        }

        public string Message { get; set; }
        public LLMEnum? LLM { get; set; }
    }
}
