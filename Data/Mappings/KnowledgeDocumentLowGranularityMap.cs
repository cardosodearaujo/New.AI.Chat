using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data.Mappings
{
    public static class KnowledgeDocumentLowGranularityMap
    {
        public static void MapKnowledgeDocumentInformationLowGranularity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnowledgeDocumentLowGranularity>(
                entity =>
                {
                    entity.ToTable("KDLowGranularity_KDLG");
                    
                    entity.HasKey(e => e.Id);
                    
                    entity.Property(e => e.Id)
                          .HasColumnName("KDLG_ID")
                          .IsRequired();
 
                    entity.Property(e => e.InformationId)
                          .HasColumnName("KDLG_InformationId")
                          .IsRequired();
 
                    entity.Property(e => e.ContentText)
                          .HasColumnName("KDLG_ContextText")
                          .IsRequired();
                    
                    entity.Property(e => e.Embedding)
                          .HasColumnName("KDLG_Embedding")
                          .HasColumnType("vector(768)")
                          .IsRequired();
                    
                    entity.HasIndex(e => e.Embedding)
                          .HasMethod("hnsw")
                          .HasOperators("vector_l2_ops");
 
                    entity.HasIndex(e => e.ContentText)
                          .HasMethod("gin")
                          .HasOperators("gin_trgm_ops");
                    
                    entity.HasOne(d => d.KnowledgeDocumentInformation)
                          .WithMany(p => p.LowGranularityList)
                          .HasForeignKey(d => d.InformationId)
                          .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
