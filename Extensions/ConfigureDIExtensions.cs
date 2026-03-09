using New.AI.Chat.Services;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Extensions
{
    public static class ConfigureDIExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IIngestionService, IngestionService>();
        }
    }
}
