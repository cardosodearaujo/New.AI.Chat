using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Extensions
{
    public static class UserAdminSeedExtension
    {
        public static async Task EnsureSeedData(this AIDbContext context, IPasswordHashService passwordHashService)
        {
            var existing = await context.DbSetUsers.FirstOrDefaultAsync(u => u.Username == "admin");
            if (existing == null)
            {
                var admin = new User
                {
                    FullName = "Administrator",
                    Email = "admin@local.com",
                    Username = "admin",
                    PasswordHash = passwordHashService.HashPassword("Abc@123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.DbSetUsers.AddAsync(admin);
            }
            else
            {
                existing.PasswordHash = passwordHashService.HashPassword("Abc@123");
                existing.IsActive = true;
                existing.UpdatedAt = DateTime.UtcNow;
                context.DbSetUsers.Update(existing);
            }

            await context.SaveChangesAsync();
        }
    }
}
