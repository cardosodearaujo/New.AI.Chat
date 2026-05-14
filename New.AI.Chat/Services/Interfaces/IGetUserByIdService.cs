using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IGetUserByIdService : IDefaultService<Guid, UserResponseDTO>
    {
    }
}
