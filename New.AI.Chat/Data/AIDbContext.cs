using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data.Mappings;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data
{
    public class AIDbContext : DbContext
    {
        public AIDbContext(DbContextOptions<AIDbContext> options) : base(options) { }
                
        public DbSet<KnowledgeDocumentInformation> DbSetKDInformation { get; set; }
        public DbSet<KnowledgeDocumentHighGranularity> DbSetKDHighGranularity { get; set; }
        public DbSet<KnowledgeDocumentLowGranularity> DbSetKDLowGranularity { get; set; }
        public DbSet<KnowledgeDocument> DbSetKnowledgeDocument { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.MapPostgresExtensions();

            //Informações gerais do documento, como nome, formato e etc.
            modelBuilder.MapKnowledgeDocumentInformation();

            //Baixa granularidade: blocos grandes de textos.
            modelBuilder.MapKnowledgeDocumentInformationHighGranularity();

            //Alta granularidade: blocos pequenos de textos.
            modelBuilder.MapKnowledgeDocumentInformationLowGranularity();

            //Tabela antiga com dados de altissima granulidade.
            modelBuilder.MapKnowledgeDocument();

            base.OnModelCreating(modelBuilder);
        }
    }
}
