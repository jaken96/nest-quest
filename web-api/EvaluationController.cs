namespace NestQuest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Criteria;
using OverpassApiModel;
using POI = PointOfInterest;

[ApiController]
[Route("api/[controller]")]
public class EvaluationController : Controller 
{
    private readonly ILogger<EvaluationController> _logger;
    public EvaluationController(ILogger<EvaluationController> logger)
    {
        _logger = logger;
    }

    //[MapToApiVersion("2.0")]
    [HttpGet, Route("criteria")]
    public async Task<object> GetCriteria(CancellationToken token, AppDbContext dbContext)
    {
        _logger.LogInformation($"GET Request made to '/criteria' endpoint");
        var criteria = await dbContext.Criteria.ToListAsync(token);
        return TypedResults.Ok(criteria);
    }

    //[MapToApiVersion("2.0")]
    [HttpPost, Route("criteria")]
    public async Task<object> PostCriteria(CancellationToken token, AppDbContext dbContext, CriteriaModel dto)
    {
        _logger.LogInformation($"POST Request made to '/criteria' endpoint");
        if (dto == null || dto.Criteria.Count == 0)
            return TypedResults.BadRequest("Criteria list is empty or null.");

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
        return TypedResults.Ok(updatedCriteria);
    }
    
    
    //[MapToApiVersion("2.0")]
    [HttpGet, Route("score")]
    public async Task<object> GetScore(CancellationToken token, AppDbContext dbContext, EvaluationService evaluationService, double lat, double lon)
    {
        _logger.LogInformation($"GET Request made to '/score' endpoint");
        var criteria = await dbContext.Criteria.ToListAsync();
        Console.WriteLine("Loading saved criteria...");
        Console.WriteLine(string.Join(", ", criteria));
        var score = await evaluationService.BinaryScore(lat, lon, criteria, token);
        // score should be a double if the task is resolved
        return TypedResults.Ok(score); 
    }
    
    //[MapToApiVersion("2.0")]
    [HttpGet, Route("score-detail")]
    public async Task<object> GetScoreDetail(CancellationToken token, AppDbContext dbContext, EvaluationService evaluationService, double lat, double lon)
    {
        _logger.LogInformation($"GET Request made to '/score-detail' endpoint");
        var criteria = await dbContext.Criteria.ToListAsync();
        Console.WriteLine("Loading saved criteria...");
        Console.WriteLine(string.Join(", ", criteria));
        var score = await evaluationService.BinaryScoreDetail(lat, lon, criteria, token);
        return TypedResults.Ok(score);
    }
}
