using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var role = auth.Authenticate(request.Username, request.Password);
        if (role is null) return Unauthorized();
        return Ok(new LoginResponse(request.Username, role));
    }
}

public record LoginRequest(string Username, string Password);
public record LoginResponse(string User, string Role);
