using New.AI.Chat.Services;
using New.AI.Chat.Services.AIServices;
using New.AI.Chat.Services.Interfaces;
using New.AI.Chat.Services.Interfaces.AIInterfaces;

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
            services.AddScoped<IFileService, FileService>();
        }

        private static void InjectAIServices(this IServiceCollection services)
        {
            services.AddScoped<ILLMStrategyFactoryService, LLMStrategyFactoryService>();
            services.AddScoped<ILLMStrategyService, GeminiFlashLLMService>();
            services.AddScoped<ILLMStrategyService, Phi3LLMService>();
            services.AddScoped<ILLMStrategyService, Qwen15LLMService>();
            services.AddScoped<ILLMStrategyService, Quen7bLLMService>();
            services.AddScoped<ILLMStrategyService, NerLLMService>();            
        }
    }
}
