namespace New.AI.Chat.Models
{
    public class AuthenticationLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime LoginDateTime { get; set; } = DateTime.UtcNow;
        public DateTime TokenExpiresAt { get; set; }
        public User? User { get; set; }
    }
}
