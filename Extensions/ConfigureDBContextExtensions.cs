using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;

namespace New.AI.Chat.Extensions
{
    public static class ConfigureDBContextExtensions
    {
        public static void AddDDBContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AIDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    o =>
                    {
                        o.UseVector();
                        o.CommandTimeout(0); 
                    }
                ));
        }
    }
}
