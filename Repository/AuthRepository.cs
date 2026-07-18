using Employee.API.Data;
using Employee.API.DTOs;
using Employee.API.Interfaces;
using Employee.API.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Employee.API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                return "User already exists.";
            }

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = registerDto.Role
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null)
                return null;

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return null;

            return new LoginResponseDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,

                // JWT token will be added in the next lesson
                Token = "",

                Expiration = DateTime.Now.AddHours(1)
            };
        }
    }
}