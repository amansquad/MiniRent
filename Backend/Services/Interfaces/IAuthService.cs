using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> RegisterAsync(RegisterUserDto registerDto, int currentUserId);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<UserDto?> UpdateProfileAsync(int userId, UpdateUserDto updateDto);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null);
}
