using New.AI.Chat.Services;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Extensions
{
    public static class ConfigureDIExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.InjectServices();
            services.InjectAIServices();
        }

        private static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IIngestionService, IngestionService>();
        }

        private static void InjectAIServices(this IServiceCollection services)
        {
            services.AddScoped<ILLMStrategyService, Phi3LLMService>();
            services.AddScoped<ILLMStrategyService, QwenLLMService>();
            services.AddScoped<ILLMStrategyService, NerLLMService>();
        }
    }
}
