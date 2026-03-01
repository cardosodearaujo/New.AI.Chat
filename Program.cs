using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using New.AI.Chat.Data;
using New.AI.Chat.Services;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            builder.Services.AddTransient<Kernel>(sp =>
            {
                var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };

                var kernelBuilder = Kernel.CreateBuilder();

                kernelBuilder.AddOpenAIChatCompletion(
                    modelId: "phi3",
                    apiKey: "ignore",
                    endpoint: new Uri("http://localhost:11434/v1"),
                    httpClient: httpClient
                );

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
