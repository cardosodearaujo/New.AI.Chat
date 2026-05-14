using System;

namespace New.AI.Chat.DTOs
{
    public class ChangePasswordRequestDTO
    {
        public Guid Id { get; set; }
        public ChangePasswordDTO Data { get; set; } = new ChangePasswordDTO();
    }
}
