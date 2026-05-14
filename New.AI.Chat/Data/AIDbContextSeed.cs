using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Data
{
    public static class AIDbContextSeed
    {
        public static async Task EnsureSeedData(this AIDbContext context, IPasswordHashService passwordHashService)
        {
            if (await context.DbSetUsers.AnyAsync()) return;

            var admin = new User
            {
                FullName = "Administrator",
                Email = "admin@local",
                Username = "admin",
                PasswordHash = passwordHashService.HashPassword("Abc@123"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.DbSetUsers.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
