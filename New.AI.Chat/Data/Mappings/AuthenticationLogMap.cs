using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data.Mappings
{
    public static class AuthenticationLogMap
    {
        public static void MapAuthenticationLog(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthenticationLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("AL_ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.UserId)
                    .HasColumnName("AL_USER_ID")
                    .IsRequired();

                entity.Property(e => e.Username)
                    .HasColumnName("AL_USERNAME")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Token)
                    .HasColumnName("AL_TOKEN")
                    .IsRequired();

                entity.Property(e => e.LoginDateTime)
                    .HasColumnName("AL_LOGIN_DATE_TIME")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TokenExpiresAt)
                    .HasColumnName("AL_TOKEN_EXPIRES_AT")
                    .IsRequired();

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.LoginDateTime);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.AuthenticationLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("AuthenticationLogs_AL");
            });
        }
    }
}
