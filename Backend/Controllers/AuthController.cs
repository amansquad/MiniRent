using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user with the Agent role.
    /// </summary>
    /// <param name="registerDto">The registration details.</param>
    /// <returns>The created user details.</returns>
    [HttpPost("self-register")]
    [AllowAnonymous]
    public async Task<IActionResult> SelfRegister([FromBody] RegisterUserDto registerDto)
    {
        _logger.LogInformation("Self-registration attempt for username: {Username}", registerDto.Username);
        
        // Force basic user role regardless of what the client sends
        registerDto.Role = "Agent";

        var result = await _authService.RegisterAsync(registerDto, Guid.Empty);

        if (result == null)
        {
            _logger.LogWarning("Self-registration failed. Username already exists: {Username}", registerDto.Username);
            return BadRequest(new { message = "Username already exists" });
        }

        _logger.LogInformation("User self-registered successfully: {Id}", result.Id);
        return CreatedAtAction(nameof(GetCurrentUser), new { id = result.Id }, result);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A token and user profile if successful.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for username: {Username}", loginDto.Username);
        var result = await _authService.LoginAsync(loginDto);

        if (result == null)
        {
            _logger.LogWarning("Login failed for username: {Username}", loginDto.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        _logger.LogInformation("User logged in successfully: {Username}", loginDto.Username);
        return Ok(result);
    }

    /// <summary>
    /// Registers a new user (Admin only).
    /// </summary>
    /// <param name="registerDto">The registration details.</param>
    /// <returns>The created user details.</returns>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        _logger.LogInformation("Admin registration attempt for username: {Username}", registerDto.Username);
        
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        
        if (userIdClaim == null || roleClaim == null)
        {
            return Unauthorized();
        }

        // Only admins can register users
        if (roleClaim.Value != "Admin")
        {
            _logger.LogWarning("Forbidden registration attempt by non-admin user: {UserId}", userIdClaim.Value);
            return StatusCode(403, new { message = "Only admins can create users" });
        }

        var currentUserId = Guid.Parse(userIdClaim.Value);
        var result = await _authService.RegisterAsync(registerDto, currentUserId);

        if (result == null)
        {
            _logger.LogWarning("Admin registration failed. Username already exists: {Username}", registerDto.Username);
            return BadRequest(new { message = "Username already exists" });
        }

        _logger.LogInformation("User registered by admin successfully: {Id}", result.Id);
        return CreatedAtAction(nameof(GetCurrentUser), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets the current authenticated user's profile.
    /// </summary>
    /// <returns>The user profile.</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _authService.GetCurrentUserAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("check-username")]
    public async Task<IActionResult> CheckUsername([FromBody] CheckUsernameDto dto)
    {
        var isUnique = await _authService.IsUsernameUniqueAsync(dto.Username, dto.ExcludeUserId);
        return Ok(new { isUnique });
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var result = await _authService.UpdateProfileAsync(userId, updateDto);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
