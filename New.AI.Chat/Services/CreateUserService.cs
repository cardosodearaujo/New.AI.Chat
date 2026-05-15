using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class CreateUserService : DefaultService<CreateUserDTO, UserResponseDTO>, ICreateUserService
    {
        private readonly AIDbContext _dbContext;
        private readonly IPasswordHashService _passwordHashService;

        public CreateUserService(AIDbContext dbContext, IPasswordHashService passwordHashService)
        {
            _dbContext = dbContext;
            _passwordHashService = passwordHashService;
        }

        protected override Task Validate(CreateUserDTO entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Username)) AddError("Username é obrigatório.");
            if (string.IsNullOrWhiteSpace(entry.Email)) AddError("Email é obrigatório.");
            if (string.IsNullOrWhiteSpace(entry.Password)) AddError("Password é obrigatório.");
            return Task.CompletedTask;
        }

        protected override async Task DoProcess(CreateUserDTO dto)
        {
            var exists = await _dbContext.DbSetUsers.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists)
            {
                AddError("Usuário com este username ou email já existe.");
                return;
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = _passwordHashService.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.DbSetUsers.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            Data = new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
