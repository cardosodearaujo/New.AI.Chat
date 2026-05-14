using System.Collections.Generic;

namespace New.AI.Chat.DTOs
{
    public class GetAuthenticationLogsResponseDTO
    {
        public List<AuthenticationLogResponseDTO> Logs { get; set; } = new List<AuthenticationLogResponseDTO>();
        public int TotalRecords { get; set; }
    }
}
