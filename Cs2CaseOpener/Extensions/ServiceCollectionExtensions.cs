using Cs2CaseOpener.Data;
using Cs2CaseOpener.Services;

namespace Cs2CaseOpener.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);

        services.AddHttpClient();
        services.AddSignalR();
        
        services.AddMemoryCache();
        services.AddControllers();

        services.AddSwaggerConfiguration();

        services.AddSingleton<DataRetentionService>();
        services.AddSingleton<CrateOpeningService>();
        
        services.ConfigureIPGeolocationService(configuration);

        services.AddScoped<AuthorizationService>();
        services.AddScoped<UnitOfWork, UnitOfWork>();
        services.AddScoped<ByMykelDataService, ByMykelDataService>();
        services.AddScoped<DatabaseInitializationService>();
        services.AddScoped<SkinService>();
        services.AddScoped<CrateService>();
        services.AddScoped<RarityService>();
        services.AddScoped<PriceService>();
        services.AddScoped<ApiScraper, ApiScraper>();

        services.AddHostedService<ScrapeApiBackgroundService>();
        services.AddHostedService(sp => sp.GetRequiredService<DataRetentionService>());
        services.AddHostedService(sp => sp.GetRequiredService<CrateOpeningService>());

        services.ConfigureApiScraper(configuration);

        return services;
    }
}