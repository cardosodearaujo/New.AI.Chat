using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using New.AI.Chat.Data;
using New.AI.Chat.Services;
using New.AI.Chat.Services.Interfaces;
using OpenAI;
using System.ClientModel;

namespace New.AI.Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IIngestionService, IngestionService>();

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            builder.Services.AddTransient<Kernel>(sp =>
            {
                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5),
                    BaseAddress = new Uri("http://localhost:11434/v1")
                };

                var kernelBuilder = Kernel.CreateBuilder();

                var opcoes = new OpenAIClientOptions
                {
                    Endpoint = new Uri("http://localhost:11434/v1"),
                    NetworkTimeout = TimeSpan.FromMinutes(240) 
                };

                var clienteOpenAi = new OpenAIClient(new ApiKeyCredential("ignore"), opcoes);

                kernelBuilder.AddOpenAIChatCompletion("phi3", clienteOpenAi);

                kernelBuilder.AddOpenAIEmbeddingGenerator("nomic-embed-text", clienteOpenAi);

                return kernelBuilder.Build();
            });

            builder.Services.AddDbContext<AIDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseVector()
                ));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "New.AI.Chat API v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
