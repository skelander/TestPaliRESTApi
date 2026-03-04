using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FeaturesController(IFeaturesService features) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(features.GetAll());

    [HttpPut("{user}")]
    public IActionResult SetUser(string user, [FromBody] SetFeatureRequest request)
    {
        features.SetEnabled(user, request.Enabled);
        return Ok();
    }
}

public record SetFeatureRequest(bool Enabled);
