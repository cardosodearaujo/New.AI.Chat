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
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace New.AI.Chat.Services
{
    public class AuthService : DefaultService<LoginDTO, AuthResponseDTO>, IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly AIDbContext _dbContext;
        private readonly IPasswordHashService _passwordHashService;
        private AuthResponseDTO? _result;

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

        protected override Task Validate(LoginDTO entry, CancellationToken cancellationToken)
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

        protected override async Task DoProcess(LoginDTO entry, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = DecodeKey(_jwtSettings.Key);
            var loginAttemptTime = DateTime.UtcNow;

            cancellationToken.ThrowIfCancellationRequested();

            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Username == entry.Username && u.IsActive, cancellationToken);

            if (user == null)
            {
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

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
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

            _result = new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = tokenDescriptor.Expires ?? loginAttemptTime.AddMinutes(_jwtSettings.ExpirationMinutes)
            };

            await LogAttempt(user.Id, user.Username, tokenString, _result.ExpiresAt);
        }

        protected override Task<AuthResponseDTO?> GetResultData(LoginDTO entry, CancellationToken cancellationToken) => Task.FromResult(_result);

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
            }
        }
    }
}
