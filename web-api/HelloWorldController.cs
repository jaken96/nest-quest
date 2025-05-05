namespace NestQuest.Services;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : Controller
{

    private readonly ILogger<HelloWorldController> _logger;
    public HelloWorldController(ILogger<HelloWorldController> logger)
    {
        _logger = logger;
    }

    [HttpGet, Route("route1")]
    public int HelloWorld(int id)
    {
        _logger.LogInformation($"Request made: {id}");
        return id;
    }
}