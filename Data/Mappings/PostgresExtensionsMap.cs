using Microsoft.EntityFrameworkCore;

namespace New.AI.Chat.Data.Mappings
{
    public static class PostgresExtensionsMap
    {
        public static void MapPostgresExtensions(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("vector");
            modelBuilder.HasPostgresExtension("pg_trgm");
        }
    }
}
