using System;

namespace New.AI.Chat.DTOs
{
    public class AuthenticationLogResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime LoginDateTime { get; set; }
        public DateTime TokenExpiresAt { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
