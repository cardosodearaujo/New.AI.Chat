using Pgvector;

namespace New.AI.Chat.Models
{
    public class KnowledgeDocumentLowGranularity
    {
        public KnowledgeDocumentLowGranularity()
        {
            Id = Guid.NewGuid();
            ContentText = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid InformationId { get; set; }
        public string ContentText { get; set; }
        public Vector? Embedding { get; set; }
        public KnowledgeDocumentInformation KnowledgeDocumentInformation { get; set; }
        public ICollection<KnowledgeDocumentHighGranularity> HighGranularityList { get; set; }
    }
}
