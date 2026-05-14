using Microsoft.EntityFrameworkCore;
using New.AI.Chat.Data;
using New.AI.Chat.DTOs;
using New.AI.Chat.Models;
using New.AI.Chat.Services.Interfaces;

namespace New.AI.Chat.Services
{
    public class UsersService : IUsersService
    {
        private readonly AIDbContext _dbContext;
        private readonly IPasswordHashService _passwordHashService;

        public UsersService(AIDbContext dbContext, IPasswordHashService passwordHashService)
        {
            _dbContext = dbContext;
            _passwordHashService = passwordHashService;
        }

        public async Task<GetUsersResponseDTO> GetAll()
        {
            var users = await _dbContext.DbSetUsers
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Username = u.Username,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return new GetUsersResponseDTO { Users = users };
        }

        public async Task<UserResponseDTO?> GetById(Guid id)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserResponseDTO> Create(CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username)) throw new InvalidOperationException("Username é obrigatório.");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new InvalidOperationException("Email é obrigatório.");
            if (string.IsNullOrWhiteSpace(dto.Password)) throw new InvalidOperationException("Password é obrigatório.");

            var exists = await _dbContext.DbSetUsers.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists) throw new InvalidOperationException("Usuário com este username ou email já existe.");

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

            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task Update(Guid id, UpdateUserDTO dto)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new InvalidOperationException($"Usuário com ID {id} não encontrado.");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.DbSetUsers.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new InvalidOperationException($"Usuário com ID {id} não encontrado.");

            _dbContext.DbSetUsers.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ChangePassword(Guid id, ChangePasswordDTO dto)
        {
            var user = await _dbContext.DbSetUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new InvalidOperationException($"Usuário com ID {id} não encontrado.");

            if (!_passwordHashService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Senha atual inválida.");

            user.PasswordHash = _passwordHashService.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.DbSetUsers.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}