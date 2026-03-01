using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IChatService: IDefaultService<PromptDTO, ResponseDTO>
    {
        Task Process(PromptDTO messageDTO);
    }
}
