using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using New.AI.Chat.Configurations;
using System.Text;

namespace New.AI.Chat.Extensions
{
    public static class ConfigureAuthExtensions
    {
        public static void AddJwtAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

            if (string.IsNullOrWhiteSpace(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT signing key is not configured. Set 'JwtSettings:Key' in configuration, user secrets or environment variables.");
            }

            byte[] key;
            // Try Base64, then hex, then fall back to UTF8 bytes
            var rawKey = jwtSettings.Key!;
            try
            {
                key = Convert.FromBase64String(rawKey);
                if (key.Length == 0)
                {
                    key = Encoding.UTF8.GetBytes(rawKey);
                }
            }
            catch (FormatException)
            {
                // try hex string (e.g. generated hex 64-char secret)
                try
                {
                    // Convert.FromHexString is available on modern .NET
                    key = Convert.FromHexString(rawKey);
                    if (key.Length == 0)
                    {
                        key = Encoding.UTF8.GetBytes(rawKey);
                    }
                }
                catch (FormatException)
                {
                    // fallback to UTF8 bytes
                    key = Encoding.UTF8.GetBytes(rawKey);
                }
            }

            if (key.Length < 16)
            {
                throw new InvalidOperationException("JWT signing key is too short. Use a key with at least 16 bytes of entropy (e.g. a 32-byte random key encoded in hex or Base64).");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

            // Make authorization required by default for all endpoints
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }
    }
}
