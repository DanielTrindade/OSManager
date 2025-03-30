using Microsoft.EntityFrameworkCore;
using OSManager.Data;
using OSManager.Models;
using System.Security.Cryptography;

namespace OSManager.Services
{
    public class UserService(AppDbContext context, IConfiguration configuration)
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> RegisterUserAsync(string username, string password, string email, string fullName, string role = "Technician")
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                FullName = fullName,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateUserAsync(int id, string email, string fullName, string? role = null, bool? isActive = null)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return false;

            user.Email = email;
            user.FullName = fullName;

            if (role != null)
                user.Role = role;

            if (isActive.HasValue)
                user.IsActive = isActive.Value;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            // Gerar token aleatório
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var token = Convert.ToBase64String(tokenBytes);

            // Salvar token e expiração
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}