using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services;
using MiniRent.Backend.Config;
using Xunit;

namespace MiniRent.Tests;

public class AuthServiceTests
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = config.CreateMapper();
        
        var myConfiguration = new Dictionary<string, string?>
        {
            {"JwtSettings:SecretKey", "SuperSecretKeyThatIsLongEnoughForHmacSha256_123!"},
            {"JwtSettings:Issuer", "MiniRent"},
            {"JwtSettings:Audience", "MiniRentUsers"},
            {"JwtSettings:ExpirationInMinutes", "60"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenUsernameIsUnique()
    {
        // Arrange
        var context = GetDbContext();
        var service = new AuthService(context, _configuration, _mapper);
        var registerDto = new RegisterUserDto
        {
            Username = "testuser",
            Password = "password123",
            FullName = "Test User",
            Email = "test@example.com"
        };

        // Act
        var result = await service.RegisterAsync(registerDto, Guid.Empty);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be(registerDto.Username);
        
        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be(registerDto.Email);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenUsernameExists()
    {
        // Arrange
        var context = GetDbContext();
        context.Users.Add(new User { Username = "existinguser", PasswordHash = "...", Email = "ex@ex.com" });
        await context.SaveChangesAsync();
        
        var service = new AuthService(context, _configuration, _mapper);
        var registerDto = new RegisterUserDto
        {
            Username = "existinguser",
            Password = "password123",
            FullName = "Existing User",
            Email = "new@example.com"
        };

        // Act
        var result = await service.RegisterAsync(registerDto, Guid.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var context = GetDbContext();
        var password = "password123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User 
        { 
            Id = Guid.NewGuid(),
            Username = "loginuser", 
            PasswordHash = passwordHash, 
            Email = "login@ex.com",
            IsActive = true,
            Role = UserRole.Agent,
            FullName = "Login User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(context, _configuration, _mapper);
        var loginDto = new LoginDto { Username = "loginuser", Password = password };

        // Act
        var result = await service.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        // Arrange
        var context = GetDbContext();
        var user = new User 
        { 
            Username = "loginuser", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"), 
            Email = "login@ex.com",
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(context, _configuration, _mapper);
        var loginDto = new LoginDto { Username = "loginuser", Password = "wrongpassword" };

        // Act
        var result = await service.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }
}
