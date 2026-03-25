using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data.Mappings
{
    public static class KnowledgeDocumentMap
    {
        public static void MapKnowledgeDocument(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnowledgeDocument>(
                entity =>
                {
                    entity.ToTable("KnowledgeDocument");

                    entity.Property(e => e.Embedding).HasColumnType("vector(768)");
                });
        }
    }
}
