using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IAuthService : IDefaultService<LoginDTO, AuthResponseDTO>
    {
    }
}
