using Cs2CaseOpener.Services;

namespace Cs2CaseOpener.Extensions;

public static class GeolocationExtensions
{
    public static IServiceCollection ConfigureIPGeolocationService(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddSingleton<IIPGeolocationService>(sp =>
        {
            var hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
            var dbPath = Path.Combine(hostEnvironment.ContentRootPath, "Data/GeoLite2-Country.mmdb");
            var logger = sp.GetRequiredService<ILogger<MaxMindIPGeolocationService>>();
            
            return new MaxMindIPGeolocationService(dbPath, logger);
        });
        
        return services;
    }
}