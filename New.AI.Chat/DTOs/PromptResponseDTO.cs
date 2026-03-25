namespace New.AI.Chat.DTOs
{
    public class PromptResponseDTO
    {
        public PromptResponseDTO()
        {
            ReferenceFiles = new List<string>();
        }

        public string Response { get; set; }
        public IList<string> ReferenceFiles { get; set; }
        public string DateTime { get; set; }
    }
}
