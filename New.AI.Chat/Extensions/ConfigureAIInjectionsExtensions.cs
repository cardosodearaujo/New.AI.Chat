using Microsoft.SemanticKernel;
using New.AI.Chat.Enumerators;
using OpenAI;
using System.ClientModel;

namespace New.AI.Chat.Extensions
{
    public static class ConfigureAIInjectionsExtensions
    {
        public static void AddAIInjection(this WebApplicationBuilder builder)
        {
            var openAiOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri("http://localhost:11434/v1"),
                NetworkTimeout = TimeSpan.FromMinutes(960)
            };

            var clienteOpenAi = new OpenAIClient(new ApiKeyCredential("ignore"), openAiOptions);

            var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.AddGoogleAIGeminiChatCompletion(
                modelId: "gemini-2.5-flash",
                apiKey: builder.Configuration["AI:Google:ApiKey"],
                serviceId: LLMEnum.GeminiFlash.GetDescription());

            kernelBuilder.AddOpenAIEmbeddingGenerator(
                modelId: "nomic-embed-text",
                openAIClient: clienteOpenAi);

            kernelBuilder.AddOpenAIChatCompletion(
                modelId: "phi3",
                openAIClient: clienteOpenAi,
                serviceId: LLMEnum.Phi3.GetDescription());

            kernelBuilder.AddOpenAIChatCompletion(
                modelId: "qwen2.5-coder:1.5b",
                openAIClient: clienteOpenAi,
                serviceId: LLMEnum.Qwen15.GetDescription());

            kernelBuilder.AddOpenAIChatCompletion(
                modelId: "qwen2.5-coder:7b",
                openAIClient: clienteOpenAi,
                serviceId: LLMEnum.Qwen7b.GetDescription());

            builder.Services.AddSingleton(kernelBuilder.Build());
        }
    }
}
