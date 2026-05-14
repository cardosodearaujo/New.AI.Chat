using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using New.AI.Chat.Configurations;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace New.AI.Chat.Services
{
    public class AuthService : DefaultService<LoginDTO, AuthResponseDTO>, IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly AIDbContext _dbContext;
        private readonly IPasswordHashService _passwordHashService;

        public AuthService(
            IOptions<JwtSettings> jwtOptions, 
            AIDbContext dbContext,
            IPasswordHashService passwordHashService)
        {
            _jwtSettings = jwtOptions.Value;
            _dbContext = dbContext;
            _passwordHashService = passwordHashService;
        }

        private static byte[] DecodeKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
                return Array.Empty<byte>();

            try
            {
                var b = Convert.FromBase64String(rawKey);
                if (b.Length > 0) return b;
            }
            catch { }

            try
            {
                var b = Convert.FromHexString(rawKey);
                if (b.Length > 0) return b;
            }
            catch { }

            return Encoding.UTF8.GetBytes(rawKey);
        }

        protected override Task Validate(LoginDTO entry)
        {
            if (entry == null)
            {
                AddError("Dados inválidos.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(entry.Username))
                    AddError("O nome do usuário é obrigatório.");

                if (string.IsNullOrWhiteSpace(entry.Password))
                    AddError("A senha é obrigatória.");
            }

            return Task.CompletedTask;
        }

        protected override async Task DoProcess(LoginDTO entry)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = DecodeKey(_jwtSettings.Key);
            var loginAttemptTime = DateTime.UtcNow;

            // Try to find the user
            var user = _dbContext.DbSetUsers.FirstOrDefault(u => u.Username == entry.Username && u.IsActive);

            if (user == null)
            {
                // Log failed attempt
                await LogAttempt(Guid.Empty, entry.Username, string.Empty, loginAttemptTime.AddMinutes(_jwtSettings.ExpirationMinutes));
                AddError("Credenciais inválidas.");
                return;
            }

            var passwordValid = _passwordHashService.VerifyPassword(entry.Password, user.PasswordHash);

            if (!passwordValid)
            {
                await LogAttempt(user.Id, user.Username, string.Empty, loginAttemptTime.AddMinutes(_jwtSettings.ExpirationMinutes));
                AddError("Credenciais inválidas.");
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = loginAttemptTime.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Data = new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = tokenDescriptor.Expires ?? loginAttemptTime.AddMinutes(_jwtSettings.ExpirationMinutes)
            };

            // Log successful authentication
            await LogAttempt(user.Id, user.Username, tokenString, Data.ExpiresAt);
        }

        private async Task LogAttempt(Guid userId, string username, string token, DateTime tokenExpiresAt)
        {
            try
            {
                var log = new AuthenticationLog
                {
                    UserId = userId,
                    Username = username ?? string.Empty,
                    Token = token ?? string.Empty,
                    LoginDateTime = DateTime.UtcNow,
                    TokenExpiresAt = tokenExpiresAt
                };

                await _dbContext.DbSetAuthenticationLogs.AddAsync(log);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // Swallow logging errors so authentication flow is not affected.
            }
        }
    }
}
