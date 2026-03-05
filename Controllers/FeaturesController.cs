using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FeaturesController(IFeaturesService features, ILogger<FeaturesController> logger) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetAll() => Ok(features.GetAll());

    [HttpPut("{user}")]
    [Authorize(Roles = "admin")]
    public IActionResult SetUser(string user, [FromBody] SetFeatureRequest request)
    {
        features.SetEnabled(user, request.Enabled);
        logger.LogInformation("Feature flag updated: user={User} enabled={Enabled}", user, request.Enabled);
        return Ok();
    }
}

public record SetFeatureRequest(bool Enabled);
