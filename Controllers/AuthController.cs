using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService auth, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var role = auth.Authenticate(request.Username, request.Password);
        if (role is null)
        {
            logger.LogWarning("Failed login attempt for username={Username}", request.Username);
            return Unauthorized();
        }
        logger.LogInformation("Successful login: username={Username} role={Role}", request.Username, role);
        return Ok(new LoginResponse(request.Username, role));
    }
}

public record LoginRequest(string Username, string Password);
public record LoginResponse(string User, string Role);
