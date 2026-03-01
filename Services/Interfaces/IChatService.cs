using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IChatService
    {
        Task<DefaultDTO<ResponseDTO>> SendMessage(PromptDTO messageDTO);
    }
}
