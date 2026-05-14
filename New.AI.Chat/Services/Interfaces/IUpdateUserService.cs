using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IUpdateUserService : IDefaultService<(Guid id, UpdateUserDTO dto), bool>
    {
    }
}
