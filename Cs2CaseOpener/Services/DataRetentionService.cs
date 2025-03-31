using Cs2CaseOpener.Data;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class DataRetentionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DataRetentionService> _logger;
    private readonly IConfiguration _configuration;

    public DataRetentionService(
        IServiceScopeFactory scopeFactory,
        ILogger<DataRetentionService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retentionTime = _configuration.GetValue<int>("DataRetention:HoursToKeep", 12);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var next4am = now.Date.AddHours(4);
                if (now > next4am)
                    next4am = next4am.AddDays(1);

                var delay = next4am - now;
                await Task.Delay(delay, stoppingToken);

                await PurgeOldData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data retention job");
            }

            await Task.Delay(TimeSpan.FromHours(retentionTime), stoppingToken);
        }
    }

    public async Task PurgeOldData(DateTime? cutoffDate = null)
    {
        _logger.LogInformation("Starting data retention job");

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var retentionTime = _configuration.GetValue<int>("DataRetention:HoursToKeep", 12);
        var valuableRarities = _configuration.GetValue<string[]>("DataRetention:ValuableRarities", ["rarity_ancient_weapon", "rarity_ancient", "exceedingly_rare"]);

        if (cutoffDate == null)
            cutoffDate = DateTime.UtcNow.AddHours(-retentionTime);

        const int batchSize = 50000;
        int totalDeleted = 0;
        int batchesProcessed = 0;

        while (true)
        {
            var itemsToDelete = await dbContext.CrateOpenings
                .Where(c => !valuableRarities
                    .Contains(c.Rarity) && c.OpenedAt < cutoffDate)
                .OrderBy(c => c.Id)
                .Take(batchSize)
                .Select(c => c.Id)
                .ToListAsync();

            if (itemsToDelete.Count < 1000)
                break;

            var placeholders = string.Join(",", itemsToDelete.Select((_, i) => $"@p{i}"));
            var parameters = itemsToDelete.Select((id, i) => new Npgsql.NpgsqlParameter($"@p{i}", id)).ToArray();
            
            var deleteCommand = $"DELETE FROM \"CrateOpenings\" WHERE \"Id\" IN ({placeholders})";
            var deleted = await dbContext.Database.ExecuteSqlRawAsync(deleteCommand, parameters);

            totalDeleted += deleted;
            batchesProcessed++;

            _logger.LogInformation("Deleted batch {BatchNum}: {Count} records", batchesProcessed, deleted);
            
            await Task.Delay(1000);
        }

        _logger.LogInformation("Data retention job completed. Deleted {Count} records", totalDeleted);
        
        if (totalDeleted > 0)
        {
            _logger.LogInformation("Running VACUUM to reclaim space...");
            await dbContext.Database.ExecuteSqlRawAsync("VACUUM ANALYZE \"CrateOpenings\"");
            _logger.LogInformation("VACUUM completed");
        }
    }
}