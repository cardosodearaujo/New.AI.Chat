namespace New.AI.Chat.Extensions
{
    public static class ConfigureSwaggerExtensions
    {
        public static void AddOpenApiCustom(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public static void MapOpenApiCustom(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "New.AI.Chat API v1");
                options.RoutePrefix = string.Empty;
                // Inject custom JS (development helper) to provide a lightweight Authorize UI
                options.InjectJavascript("/swagger-custom.js");
            });
        }
    }
}
