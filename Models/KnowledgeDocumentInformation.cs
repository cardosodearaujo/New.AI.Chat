namespace New.AI.Chat.Models
{
    public class KnowledgeDocumentInformation
    {
        public KnowledgeDocumentInformation()
        {
            Id = Guid.NewGuid();
            FileName = string.Empty;
            Format = string.Empty;
            Size = 0;
            ContextText = string.Empty;
            LowGranularityList = new List<KnowledgeDocumentLowGranularity>();
        }

        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
        public string ContextText { get; set; }
        public DateTime DateFirstInsertion { get; set; }
        public DateTime DateLastInsertion { get; set; }
        public ICollection<KnowledgeDocumentLowGranularity> LowGranularityList { get; set; }
    }
}
