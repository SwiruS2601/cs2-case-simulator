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

        services.AddSingleton<DataRetentionService>();
        services.AddSingleton<CrateOpeningService>();

        services.AddSwaggerConfiguration();

        services.AddScoped<AuthorizationService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IByMykelDataService, ByMykelDataService>();
        services.AddScoped<DatabaseInitializationService>();
        services.AddScoped<SkinService>();
        services.AddScoped<CrateService>();
        services.AddScoped<RarityService>();
        services.AddScoped<PriceService>();
        services.AddScoped<IApiScraper, ApiScraper>();

        services.AddHostedService<ScrapeApiBackgroundService>();
        services.AddHostedService(sp => sp.GetRequiredService<DataRetentionService>());
        services.AddHostedService(sp => sp.GetRequiredService<CrateOpeningService>());

        services.ConfigureApiScraper(configuration);

        return services;
    }
}
