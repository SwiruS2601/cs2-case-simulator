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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PurgeOldData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data retention job");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task PurgeOldData()
    {
        _logger.LogInformation("Starting data retention job");

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var retentionDays = _configuration.GetValue<int>("DataRetention:DaysToKeep", 7);
        var valuableRarities = _configuration.GetValue<string[]>("DataRetention:ValuableRarities", ["rarity_ancient_weapon", "rarity_ancient", "exceedingly_rare"]);
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        const int batchSize = 50000;
        int totalDeleted = 0;
        int batchesProcessed = 0;

        while (true)
        {
            var itemsToDelete = await dbContext.CrateOpenings
                .Where(c => !valuableRarities.Contains(c.Rarity) && c.OpenedAt < cutoffDate)
                .OrderBy(c => c.Id)
                .Take(batchSize)
                .Select(c => c.Id)
                .ToListAsync();

            if (itemsToDelete.Count == 0)
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
    }
}