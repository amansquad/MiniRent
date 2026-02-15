using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string Phone { get; set; } = string.Empty;
}

public class RegisterUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string Phone { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

public class CheckUsernameDto
{
    public string Username { get; set; } = string.Empty;
    public int? ExcludeUserId { get; set; }
}

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string Phone { get; set; } = string.Empty;
}

public class UpdateUserByAdminDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
