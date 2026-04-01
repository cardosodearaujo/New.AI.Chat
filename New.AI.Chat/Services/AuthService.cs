using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using New.AI.Chat.Configurations;
using New.AI.Chat.DTOs;
using New.AI.Chat.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace New.AI.Chat.Services
{
    public class AuthService : DefaultService<LoginDTO, AuthResponseDTO>, IAuthService
    {
        private readonly JwtSettings _jwtSettings;

        // In-memory users (username -> password). In production replace with DB or identity provider.
        private readonly Dictionary<string, string> _users = new()
        {
            { "admin", "P@ssw0rd" },
            { "user", "password" }
        };

        public AuthService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        protected override Task Validate(LoginDTO entry)
        {
            if (entry == null)
            {
                AddError("Payload inválido.");
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(entry.Username)) AddError("O nome do utilizador é obrigatório.");
            if (string.IsNullOrWhiteSpace(entry.Password)) AddError("A password é obrigatória.");

            return Task.CompletedTask;
        }

        protected override Task DoProcess(LoginDTO entry)
        {
            // valida in-memory
            if (!_users.TryGetValue(entry.Username, out var storedPassword) || storedPassword != entry.Password)
            {
                AddError("Credenciais inválidas.");
                return Task.CompletedTask;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, entry.Username),
                new Claim(ClaimTypes.NameIdentifier, entry.Username),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Data = new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)
            };

            return Task.CompletedTask;
        }
    }
}
