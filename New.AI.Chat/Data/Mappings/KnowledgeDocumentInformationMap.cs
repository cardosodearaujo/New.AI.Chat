using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data.Mappings
{
    public static class KnowledgeDocumentInformationMap
    {
        public static void MapKnowledgeDocumentInformation(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnowledgeDocumentInformation>(
                entity =>
                {
                    entity.ToTable("KDInformation_KDI");

                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.Id)
                          .HasColumnName("KDI_ID")
                          .IsRequired();

                    entity.Property(e => e.FileName)
                          .HasColumnName("KDI_FileName")
                          .IsRequired();

                    entity.Property(e => e.Format)
                          .HasColumnName("KDI_Format")
                          .IsRequired();

                    entity.Property(e => e.Size)
                          .HasColumnName("KDI_Size")
                          .IsRequired();

                    entity.Property(e => e.ContextText)
                          .HasColumnName("KDI_ContextText")
                          .IsRequired();

                    entity.Property(e => e.DateFirstInsertion)
                          .HasColumnName("KDI_DateFirstInsertion")
                          .IsRequired();

                    entity.Property(e => e.DateLastInsertion)
                          .HasColumnName("KDI_DateLastInsertion")
                          .IsRequired();

                });
        }
    }
}
