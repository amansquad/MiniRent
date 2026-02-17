using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
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

/// <summary>
/// Data transfer object for user registration.
/// </summary>
public class RegisterUserDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(15)]
    [RegularExpression(@"^\+?(\d{1,3})?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?$", ErrorMessage = "Invalid phone number format.")]
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for user login.
/// </summary>
public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
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
    public Guid? ExcludeUserId { get; set; }
}

public class UpdateUserDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(15)]
    [RegularExpression(@"^\+?(\d{1,3})?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?$", ErrorMessage = "Invalid phone number format.")]
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
