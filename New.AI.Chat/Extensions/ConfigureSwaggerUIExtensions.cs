namespace New.AI.Chat.Extensions
{
    public static class ConfigureSwaggerUIExtensions
    {
        public static void UseSwaggerUI(this WebApplication app)
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "New.AI.Chat API v1");
                options.RoutePrefix = string.Empty;
            });
        }
    }
}
