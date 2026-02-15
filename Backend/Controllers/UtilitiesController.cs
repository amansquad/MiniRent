using Microsoft.AspNetCore.Mvc;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilitiesController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        return Ok(new { 
            version = "1.0.0", 
            buildDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        });
    }
}
