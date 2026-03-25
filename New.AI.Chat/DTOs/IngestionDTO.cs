namespace New.AI.Chat.DTOs
{
    public class IngestionDTO
    {
        public IngestionDTO()
        {
            IngestionFiles = new List<IngestionFileDTO>();
        }

        public IList<IngestionFileDTO> IngestionFiles { get; set; }
    }
}
