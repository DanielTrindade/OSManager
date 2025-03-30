using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OSManager.Data;
using OSManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OSManager.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration, UserService userService)
    {
        public async Task<(string token, User user)?> AuthenticateAsync(string username, string password)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            // Atualizar último login
            await userService.UpdateLastLoginAsync(user.Id);

            var token = GenerateJwtToken(user);
            return (token, user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment12345678");
            var issuer = configuration["Jwt:Issuer"] ?? "OSManager";
            var audience = configuration["Jwt:Audience"] ?? "OSManagerApi";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role),
            new("FullName", user.FullName),
            new("Email", user.Email)
                ]),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}