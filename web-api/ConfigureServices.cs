using Criteria;
using Microsoft.EntityFrameworkCore;
using NestQuest.Services;
using OverpassApiModel;
using POI = PointOfInterest;

public class Startup
{
    // I don't know what I'm doing, but I want to know what's wrong with this design
    // Theory:  Dependency Injection is a "dangerous" operation and we have to avoid directly injecting unecessary stuff
    //          Sooooo we should have a separate class for controllers vs startup

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration
    }
    public IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
        // This method should not have app = builder.Build()
        //              Should be broken off into Configure
        services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=nq.db"));
        services.AddScoped<CacheService<OverpassApiResponse>>();
        services.AddTransient<OverpassService>();
        services.AddTransient<EvaluationService>();
        services.AddSingleton(new RateLimiter(1, TimeSpan.FromSeconds(1)));

        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });
        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(); // what does this do?
        services.ConfigureOptions<ConfigureSwaggerOptions>(); // this should trigger something in configureSwaggerOptions
    }
    public void Configure(IApplicationBuilder app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        app.UseSwagger();
        app.UseSwaggerUI(); // this needs to take an Options parameter now?  I don't know if this is used in final product
    }
}