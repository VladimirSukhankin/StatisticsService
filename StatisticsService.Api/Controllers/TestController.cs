using Microsoft.AspNetCore.Mvc;

namespace StatisticsService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpOptions]
    public string Test()
    {
        return "this is test";
    }
}