using Microsoft.Extensions.Options;

namespace Cs2CaseOpener.Services;

public class ApiScraperConfiguration
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan? ScheduledTime { get; set; } = TimeSpan.FromHours(3); // 3 AM by default
    public bool EnableScraping { get; set; } = true;
}

public class ScrapeApiBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ApiScraperConfiguration _config;
    private readonly ILogger<ScrapeApiBackgroundService> _logger;
    private DateTime _nextRunTime;

    public ScrapeApiBackgroundService(
        IServiceScopeFactory scopeFactory, 
        IOptions<ApiScraperConfiguration> config,
        ILogger<ScrapeApiBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config.Value;
        _logger = logger;
        
        _nextRunTime = CalculateNextRunTime(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.EnableScraping)
        {
            _logger.LogInformation("API scraping is disabled");
            return;
        }

        _logger.LogInformation("API scraper scheduled with interval: {Interval}, next run at: {NextRun}", 
            _config.Interval, _nextRunTime);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                
                if (now >= _nextRunTime)
                {
                    _logger.LogInformation("Starting scheduled API scrape");
                    await RunScraperAsync();
                    
                    _nextRunTime = CalculateNextRunTime(now);
                    
                    _logger.LogInformation("Next API scrape scheduled at {Time}", _nextRunTime);
                }
                
                var timeUntilNextRun = _nextRunTime - DateTime.Now;
                var delayTime = timeUntilNextRun > TimeSpan.FromSeconds(30) 
                    ? TimeSpan.FromSeconds(30) 
                    : timeUntilNextRun;
                
                if (delayTime.TotalMilliseconds > 0)
                {
                    await Task.Delay(delayTime, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in API scraper background service");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private DateTime CalculateNextRunTime(DateTime fromTime)
    {
        if (_config.ScheduledTime.HasValue)
        {
            var scheduledTimeOfDay = _config.ScheduledTime.Value;
            var nextRun = new DateTime(
                fromTime.Year, fromTime.Month, fromTime.Day,
                scheduledTimeOfDay.Hours, scheduledTimeOfDay.Minutes, scheduledTimeOfDay.Seconds);
            
            if (fromTime > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }
            
            return nextRun;
        }
        else
        {
            return fromTime.Add(_config.Interval);
        }
    }

    private async Task RunScraperAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var apiScraper = scope.ServiceProvider.GetRequiredService<ApiScraper>();
        
        await apiScraper.ScrapeApiWithMonitoringAsync();
    }
}
