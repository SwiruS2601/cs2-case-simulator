using Cs2CaseOpener.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cs2CaseOpener.BackgroundServices;

public class ScrapeApiBackgroundService : BackgroundService
{
    private readonly ILogger<ScrapeApiBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ScrapeApiBackgroundService(ILogger<ScrapeApiBackgroundService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var targetTime = DateTime.Today.AddHours(3);
        if (now >= targetTime)
        {
            targetTime = targetTime.AddDays(1);
        }
        var initialDelay = targetTime - now;

        _logger.LogInformation("Delaying initial API scrape by {Delay} until {RunTime}", initialDelay, targetTime);
                
        await Task.Delay(initialDelay, stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var scraper = scope.ServiceProvider.GetRequiredService<ApiScraper>();
                try
                {
                    _logger.LogInformation("Scraping API...");
                    await scraper.ScrapeApi();
                    _logger.LogInformation("API scraped successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while scraping the API");
                }
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}