using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/v1")]
public class Home : ControllerBase
{
    [HttpGet("home")]
    public IActionResult Get()
    {
        return Ok("Home controller");
    }
}