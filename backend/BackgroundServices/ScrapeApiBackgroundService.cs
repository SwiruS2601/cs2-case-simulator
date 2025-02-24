using Cs2CaseOpener.Services;

namespace Cs2CaseOpener.BackgroundServices;

public class ScrapeApiBackgroundService : BackgroundService
{
    private readonly ILogger<ScrapeApiBackgroundService> _logger;
    private readonly ApiScraper _apiScraper;

    public ScrapeApiBackgroundService(ILogger<ScrapeApiBackgroundService> logger, ApiScraper apiScraper)
    {
        _logger = logger;
        _apiScraper = apiScraper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Scraping API...");
                await _apiScraper.ScrapeApi();
                _logger.LogInformation("API scraped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while scraping the API");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}