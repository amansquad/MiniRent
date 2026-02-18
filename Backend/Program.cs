using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniRent.Backend.Data;
using MiniRent.Backend.Services;
using MiniRent.Backend.Services.Interfaces;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using MiniRent.Backend.Models;
using MiniRent.Backend.Middleware;

// AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.MaxDepth = 64; // Increase depth for nested objects
        // Allow large responses for Base64 images
        options.JsonSerializerOptions.DefaultBufferSize = 10 * 1024 * 1024; // 10MB buffer
    });

// Configure Kestrel to handle large requests/responses
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
    options.Limits.MaxResponseBufferSize = 10 * 1024 * 1024; // 10MB
});

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "MiniRent API",
        Version = "v1",
        Description = "A comprehensive property rental management system API"
    });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Resolve conflicts by using full names
    c.CustomSchemaIds(type => type.FullName);

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // React/Vite dev servers
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IInquiryService, InquiryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IOwnershipService, SimpleOwnershipService>(); // Using simple version that doesn't require migration
builder.Services.AddScoped<OwnershipTableService>(); // For managing UserPropertyOwnership table

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();


app.UseResponseCompression();
app.UseResponseCaching();

// Configure Swagger (available in all environments for API documentation)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniRent API v1");
    c.RoutePrefix = "swagger"; // Serve Swagger at /swagger
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Diagnostic endpoint to check and create admin user
app.MapGet("/api/debug/check-admin", async (AppDbContext context) =>
{
    var admin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
    
    if (admin == null)
    {
        return Results.Json(new { exists = false, message = "Admin user does not exist" });
    }
    
    return Results.Json(new
    {
        exists = true,
        id = admin.Id,
        username = admin.Username,
        role = admin.Role.ToString(),
        isActive = admin.IsActive,
        email = admin.Email
    });
});

app.MapPost("/api/debug/create-admin", async (AppDbContext context) =>
{
    var adminExists = await context.Users.AnyAsync(u => u.Username == "admin");
    
    if (adminExists)
    {
        return Results.Json(new { success = false, message = "Admin user already exists" });
    }
    
    var adminUser = new User
    {
        Username = "admin",
        FullName = "System Administrator",
        Email = "admin@minirent.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
        Role = UserRole.Admin,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
    
    context.Users.Add(adminUser);
    await context.SaveChangesAsync();
    
    return Results.Json(new { success = true, message = "Admin user created successfully", username = "admin", password = "admin123" });
});

app.MapPost("/api/debug/reset-admin-password", async (AppDbContext context) =>
{
    var admin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
    
    if (admin == null)
    {
        return Results.Json(new { success = false, message = "Admin user does not exist" });
    }
    
    admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
    admin.UpdatedAt = DateTime.UtcNow;
    await context.SaveChangesAsync();
    
    return Results.Json(new { success = true, message = "Admin password reset to 'admin123'" });
});

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();

    // Seed admin user if no users exist
    if (!await context.Users.AnyAsync())
    {
        var adminUser = new User
        {
            Username = "admin",
            FullName = "System Administrator",
            Email = "admin@minirent.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
    }
}

app.Run();
