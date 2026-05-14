using Microsoft.Extensions.DependencyInjection;
using New.AI.Chat.Data;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Extensions
{
    public static class SeedExtensions
    {
        public static WebApplication UseSeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AIDbContext>();
                var passwordService = services.GetRequiredService<IPasswordHashService>();
                context.EnsureSeedData(passwordService).GetAwaiter().GetResult();
            }
            catch
            {
                // ignore seed errors
            }

            return app;
        }
    }
}
