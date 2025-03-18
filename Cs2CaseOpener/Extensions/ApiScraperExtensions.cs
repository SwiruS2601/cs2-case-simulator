using Cs2CaseOpener.Services;

namespace Cs2CaseOpener.Extensions;

public static class ApiScraperExtensions
{
    public static IServiceCollection ConfigureApiScraper(this IServiceCollection services, IConfiguration configuration)
    {
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
}