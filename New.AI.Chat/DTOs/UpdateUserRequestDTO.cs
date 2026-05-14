using System;

namespace New.AI.Chat.DTOs
{
    public class UpdateUserRequestDTO
    {
        public Guid Id { get; set; }
        public UpdateUserDTO Data { get; set; } = new UpdateUserDTO();
    }
}
