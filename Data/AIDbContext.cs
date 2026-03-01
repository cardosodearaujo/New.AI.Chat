using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data
{
    public class AIDbContext: DbContext
    {
        public AIDbContext(DbContextOptions<AIDbContext> options) : base(options) { }

        public DbSet<KnowledgeDocument> KnowledgeDocumentDbSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<KnowledgeDocument>(entity =>
            {
                entity.ToTable("KnowledgeDocument");

                entity.Property(e => e.Embedding).HasColumnType("vector(768)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
