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

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("self-register")]
    [AllowAnonymous]
    public async Task<IActionResult> SelfRegister([FromBody] RegisterUserDto registerDto)
    {
        // Force basic user role regardless of what the client sends
        registerDto.Role = "Agent";

        var result = await _authService.RegisterAsync(registerDto, 0);

        if (result == null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        return CreatedAtAction(nameof(GetCurrentUser), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(result);
    }

    [HttpPost("register")]
    [Authorize]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        
        if (userIdClaim == null || roleClaim == null)
        {
            return Unauthorized();
        }

        // Only admins can register users
        if (roleClaim.Value != "Admin")
        {
            return StatusCode(403, new { message = "Only admins can create users" });
        }

        var currentUserId = int.Parse(userIdClaim.Value);
        var result = await _authService.RegisterAsync(registerDto, currentUserId);

        if (result == null)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        return CreatedAtAction(nameof(GetCurrentUser), new { id = result.Id }, result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
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

        var userId = int.Parse(userIdClaim.Value);
        var result = await _authService.UpdateProfileAsync(userId, updateDto);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
