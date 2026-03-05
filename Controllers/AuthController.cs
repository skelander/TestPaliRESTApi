using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService auth, IConfiguration config, ILogger<AuthController> logger) : ControllerBase
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, role),
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new LoginResponse(request.Username, role, new JwtSecurityTokenHandler().WriteToken(token)));
    }
}

public record LoginRequest(string Username, string Password);
public record LoginResponse(string User, string Role, string Token);
