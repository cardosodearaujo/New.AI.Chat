using New.AI.Chat.DTOs;

namespace New.AI.Chat.Services.Interfaces
{
    public interface IUsersService
    {
        Task<GetUsersResponseDTO> GetAll();
        Task<UserResponseDTO?> GetById(Guid id);
        Task<UserResponseDTO> Create(CreateUserDTO dto);
        Task Update(Guid id, UpdateUserDTO dto);
        Task Delete(Guid id);
        Task ChangePassword(Guid id, ChangePasswordDTO dto);
    }
}
