using Cs2CaseOpener.Data;
using Cs2CaseOpener.Interfaces;
using Cs2CaseOpener.Services;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddControllers();

        services.AddHostedService<ScrapeApiBackgroundService>();
        
        services.AddSingleton<CrateOpeningService>();
        services.AddHostedService(sp => sp.GetRequiredService<CrateOpeningService>());

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IByMykelDataService, ByMykelDataService>();
        services.AddScoped<DatabaseInitializationService>();
        services.AddScoped<SkinService>();
        services.AddScoped<CrateService>();
        services.AddScoped<RarityService>();
        services.AddScoped<PriceService>();
        services.AddScoped<IApiScraper, ApiScraper>();

        services.Configure<ApiScraperConfiguration>(options => 
        {
            var scheduledTimeValue = configuration.GetValue<string>("ApiScraper:ScheduledTime");
            options.ScheduledTime = string.IsNullOrEmpty(scheduledTimeValue)
                ? null 
                : TimeSpan.Parse(scheduledTimeValue);
            options.EnableScraping = configuration.GetValue("ApiScraper:EnableScraping", true);
            var intervalValue = configuration.GetValue<string>("ApiScraper:Interval");
            options.Interval = string.IsNullOrEmpty(intervalValue)
                ? TimeSpan.FromDays(1)
                : TimeSpan.Parse(intervalValue);
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
            {
                policy.WithOrigins(
                    "http://localhost", 
                    "http://localhost:3000", 
                    "http://localhost:5173", 
                    "http://10.10.10.46:5020", 
                    "https://case.oki.gg"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });
        
        return services;
    }
}
