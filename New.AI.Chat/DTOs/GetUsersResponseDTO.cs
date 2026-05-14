using System.Collections.Generic;

namespace New.AI.Chat.DTOs
{
    public class GetUsersResponseDTO
    {
        public List<UserResponseDTO> Users { get; set; } = new List<UserResponseDTO>();
    }
}
