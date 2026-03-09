using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;

namespace New.AI.Chat.Extensions
{
    public static class ConfigureAIInjectionsExtensions
    {
        public static void AddAIInjection(this IServiceCollection services)
        {
            services.AddTransient<Kernel>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();
                var openAiOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri("http://localhost:11434/v1"),
                    NetworkTimeout = TimeSpan.FromMinutes(960)
                };
                var clienteOpenAi = new OpenAIClient(new ApiKeyCredential("ignore"), openAiOptions);
                kernelBuilder.AddOpenAIChatCompletion("phi3", clienteOpenAi);
                kernelBuilder.AddOpenAIEmbeddingGenerator("nomic-embed-text", clienteOpenAi);

                return kernelBuilder.Build();
            });
        }
    }
}
