using New.AI.Chat.Extensions;
using New.AI.Chat.Middleware;

namespace New.AI.Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureApp(
                ConfigureBuilder(args)
                ).Run();
        }

        public static WebApplication ConfigureBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDependencyInjection();
            builder.Services.AddControllers();
            builder.Services.AddOpenApiCustom();
            builder.AddAIInjection();
            builder.AddDDBContext();
            builder.AddJwtAuthentication();
            return builder.Build();
        }

        private static WebApplication ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseStaticFiles();
                app.MapOpenApiCustom();
            }
            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSeedData();

            return app;
        }
    }
}
