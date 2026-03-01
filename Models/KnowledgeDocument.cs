using Pgvector;

namespace New.AI.Chat.Models
{
    public class KnowledgeDocument
    {
        public KnowledgeDocument()
        {
            ContentText = string.Empty;
            Font = string.Empty;
        }

        public int Id { get; set; }
        public string ContentText { get; set; }
        public string Font { get; set; }
        public Vector? Embedding { get; set; }
    }
}
