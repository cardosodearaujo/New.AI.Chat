using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;

namespace New.AI.Chat.Data.Mappings
{
    public static class UserMap
    {
        public static void MapUser(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("USR_ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FullName)
                    .HasColumnName("USR_FULL_NAME")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .HasColumnName("USR_EMAIL")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .HasColumnName("USR_USERNAME")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("USR_PASSWORD_HASH")
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasColumnName("USR_IS_ACTIVE")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("USR_CREATED_AT")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("USR_UPDATED_AT");

                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.IsActive);

                entity.HasMany(e => e.AuthenticationLogs)
                    .WithOne(al => al.User)
                    .HasForeignKey(al => al.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Users_USR");
            });
        }
    }
}
