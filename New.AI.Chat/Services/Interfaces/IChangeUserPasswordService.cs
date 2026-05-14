using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IChangeUserPasswordService : IDefaultService<(Guid id, ChangePasswordDTO dto), bool>
    {
    }
}
