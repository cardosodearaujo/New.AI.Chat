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
            builder.Services.AddOpenApiCustom();
            builder.AddAIInjection();
            builder.AddDDBContext();
            builder.AddJwtAuthentication();
            return builder;
        }

        private static WebApplication ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseStaticFiles();
                app.MapOpenApiCustom();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}
