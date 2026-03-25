using Pgvector;

namespace New.AI.Chat.Models
{
    public class KnowledgeDocumentHighGranularity
    {
        public KnowledgeDocumentHighGranularity()
        {
            Id = Guid.NewGuid();
            ContentText = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid LowGranularityId { get; set; }
        public string ContentText { get; set; }
        public Vector? Embedding { get; set; }
        public KnowledgeDocumentLowGranularity KnowledgeDocumentLowGranularity { get; set; }
    }
}
