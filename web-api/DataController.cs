namespace NestQuest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Criteria;
using OverpassApiModel;
using POI = PointOfInterest;

[ApiController]
[Route("api/[controller]")]
//[ApiVersion("2.0")] // circle back to this
//[Route("api/v{version:apiVersion}/[controller]")]
public class DataController : Controller
{
    private readonly ILogger<DataController> _logger;
    public DataController(ILogger<DataController> logger)
    {
      _logger = logger;
    }

    //[MapToApiVersion("2.0")]
    [HttpGet, Route("homes")]
    public IResult GetHomes(double minLon, double minLat, double maxLon, double maxLat)
    {
        _logger.LogInformation($"GET Request made to '/homes' endpoint");
        var homes = new List<POI.Home>
        {
            new("Big Ben", 51.5052345, -0.09),
            new("Tesco Express", 51.5065, -0.095),
            new("Heathrow Airport", 51.51, -0.1),
            new("British Library", 51.49, -0.08),
            new("London Library", 51.511, -0.09),
            new("Hyde Park", 51.5075, -0.07),
            new("Regent's Park", 51.52, -0.1),
            new("King's College London", 51.52, -0.12),
            new("Sainsbury's Local", 51.49, -0.06),
            new("Waitrose & Partners", 51.52, -0.095),
            new("Random Flat A", 51.48, -0.1),
            new("Green Park", 51.513, -0.11),
            new("Baker Street 221B", 51.515, -0.08),
            new("Camden Town", 51.507, -0.08),
            new("Tower of London", 51.49, -0.11)
        };
        var filteredHomes = homes.Where(h => 
            h.Lat >= minLat && h.Lat <= maxLat &&
            h.Lon >= minLon && h.Lon <= maxLon
        ).ToList();

        return TypedResults.Ok(filteredHomes);
    }

    //[MapToApiVersion("2.0")]
    [HttpGet, Route("poi")]
    public async Task<object> GetPoi(CancellationToken token, OverpassService overpassService, POI.Category cat, double minLon, double minLat, double maxLon, double maxLat)
    {
        _logger.LogInformation($"GET Request made to '/poi' endpoint");
        var poi = await overpassService.GetPoiByCategoryAndBbox(cat, minLon, minLat, maxLon, maxLat, token);
        return TypedResults.Ok(poi);
    }


}
