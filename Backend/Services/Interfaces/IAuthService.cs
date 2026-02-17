using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> RegisterAsync(RegisterUserDto registerDto, Guid currentUserId);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
    Task<UserDto?> UpdateProfileAsync(Guid userId, UpdateUserDto updateDto);
    Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeUserId = null);
}
