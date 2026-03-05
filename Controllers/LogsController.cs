using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController(LogStore store) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult GetAll() => Ok(store.GetAll());
}
