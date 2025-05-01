using Criteria;
using Microsoft.EntityFrameworkCore;
using NestQuest.Services;
using OverpassApiModel;
using POI = PointOfInterest;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class NestController : ControllerBase
{
    public NestController(ILogger<NestController> logger, INestRepository data)
    {
        // Honestly, not a clue if this is needed.
        // This is from an example that doesn't fully tie out on its own

        // Ahah!  I need to create a controller for each endpoint identified previously:
        //      homes, attractions, poi, criteria (GET), criteria (POST), score, score-detail
        // These controller methods will contain the logic and
    }

    // 1 - GET "api/v2.0/homes"
    [MapToApiVersion("2.0")]
    [HttpGet, Route("homes")]
    public HomesController(double minLon, double minLat, double maxLon, double maxLat)
    {
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

        return Results.Ok(filteredHomes);
    }

    // 2 - GET "api/v2.0/attractions"
    [MapToApiVersion("2.0")]
    [HttpGet, Route("attractions")]
    public async AttractionsController(CancellationToken token, OverpassService overpassService, double minLon, double minLat, double maxLon, double maxLat)
    {
            var attractionLocations = new List<object>
    {
        new { location = new[] { 51.506, -0.095 }, type = "Grocery" },
        new { location = new[] { 51.51, -0.08 }, type = "Airport" },
        new { location = new[] { 51.52, -0.09 }, type = "Library" },
        new { location = new[] { 51.507, -0.07 }, type = "Park" },
        new { location = new[] { 51.52, -0.12 }, type = "School" },
        new { location = new[] { 51.49, -0.06 }, type = "Grocery" },
        new { location = new[] { 51.511, -0.1 }, type = "Library" },
        new { location = new[] { 51.513, -0.11 }, type = "Park" }
    };

    return Results.Ok(attractionLocations);
    }

    // 3 - GET "api/v2.0/poi"
    [MapToApiVersion("2.0")]
    [HttpGet, Route("poi")]
    public async PoiController(CancellationToken token, OverpassService overpassService, POI.Category cat, double minLon, double minLat, double maxLon, double maxLat)
    {
        var poi = await overpassService.GetPoiByCategoryAndBbox(cat, minLon, minLat, maxLon, maxLat, token);
        return Results.Ok(poi);
    }

    // 4 - GET "api/v2.0/criteria"
    [MapToApiVersion("2.0")]
    [HttpGet, Route("criteria")]
    public async CriteriaController(CancellationToken token, AppDbContext dbContext)
    {
        var criteria = await dbContext.Criteria.ToListAsync(token);
        return Results.Ok(criteria);
    }

    // 5 - POST "api/v2.0/criteria"
    [MapToApiVersion("2.0")]
    [HttpPost, Route("criteria")]
    public async CriteriaController(CancellationToken token, AppDbContext dbContext, CriteriaModel dto)
    {
        if (dto == null || dto.Criteria.Count == 0)
            return Results.BadRequest("Criteria list is empty or null.");

        foreach (var criterion in dto.Criteria)
        {
            var existingCriterion = await dbContext.Criteria.FindAsync(criterion.Id, token);
            if (existingCriterion != null)
                dbContext.Entry(existingCriterion).CurrentValues.SetValues(criterion);
            else
                dbContext.Criteria.Add(criterion);
        }
        await dbContext.SaveChangesAsync(token);
        var ids = dto.Criteria.Select(c => c.Id).ToList();
        var updatedCriteria = await dbContext.Criteria
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(token);
        return Results.Ok(updatedCriteria);
    }
    // 6 - GET "api/v2.0/score"
    [MapToApiVersion("2.0")]
    [HttpGet, Route("score")]
    public async ScoreController(CancellationToken token, AppDbContext dbContext, EvaluationService evaluationService, double lat, double lon)
    {
        var criteria = await dbContext.Criteria.ToListAsync();
        Console.WriteLine("Loading saved criteria...");
        Console.WriteLine(string.Join(", ", criteria));
        var score = await evaluationService.BinaryScore(lat, lon, criteria, token);
        return Results.Ok(score);
    }
    
    // 7 - GET "api/v2.0/score-detail"
    [MapToApiVersion("2.0")]
    [HttpGet, Route("score-detail")]
    public async ScoreDetailController(CancellationToken token, AppDbContext dbContext, EvaluationService evaluationService, double lat, double lon)
    {
        var criteria = await dbContext.Criteria.ToListAsync();
        Console.WriteLine("Loading saved criteria...");
        Console.WriteLine(string.Join(", ", criteria));
        var score = await evaluationService.BinaryScoreDetail(lat, lon, criteria, token);
        return Results.Ok(score);
    }
}