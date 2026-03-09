using New.AI.Chat.Extensions;

namespace New.AI.Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureApp(ConfigureBuilder(args).Build()).Run();
        }

        public static WebApplicationBuilder ConfigureBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDependencyInjection();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddAIInjection();
            builder.AddDDBContext();
            return builder;
        }

        private static WebApplication ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}
