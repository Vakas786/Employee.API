using Employee.API.DTOs;

namespace Employee.API.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> RegisterAsync(RegisterDto registerDto);

        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    }
}