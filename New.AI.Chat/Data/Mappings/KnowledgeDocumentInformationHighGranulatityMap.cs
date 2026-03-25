using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data.Mappings
{
    public static class KnowledgeDocumentInformationHighGranulatityMap
    {
        public static void MapKnowledgeDocumentInformationHighGranularity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnowledgeDocumentHighGranularity>(
                entity =>
                {
                    entity.ToTable("KDHighGranularity_KDHG");

                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.Id)
                          .HasColumnName("KDHG_ID")
                          .IsRequired();

                    entity.Property(e => e.LowGranularityId)
                          .HasColumnName("KDHG_LowGranularityId")
                           .IsRequired();

                    entity.Property(e => e.ContentText)
                          .HasColumnName("KDHG_ContentText")
                          .IsRequired();
                    
                    entity.Property(e => e.Embedding)
                          .HasColumnName("KDHG_Embedding")
                          .HasColumnType("vector(768)")
                          .IsRequired();
                    
                    entity.HasIndex(e => e.Embedding)
                          .HasMethod("hnsw")
                          .HasOperators("vector_l2_ops");

                    entity.HasIndex(e => e.ContentText)
                          .HasMethod("gin")
                          .HasOperators("gin_trgm_ops");

                    entity.HasOne(d => d.KnowledgeDocumentLowGranularity)
                          .WithMany(p => p.HighGranularityList)
                          .HasForeignKey(d => d.LowGranularityId)
                          .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
