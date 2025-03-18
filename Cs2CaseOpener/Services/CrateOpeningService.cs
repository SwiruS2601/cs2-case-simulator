using System.Collections.Concurrent;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class CrateOpeningService : BackgroundService
{
    private readonly ConcurrentQueue<CrateOpening> _openingsBuffer = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CrateOpeningService> _logger;
    private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(10);
    private readonly int _batchSize = 1000;
    private static readonly SemaphoreSlim _counterSemaphore = new(1, 1);
    
    public CrateOpeningService(
        IServiceScopeFactory scopeFactory,
        ILogger<CrateOpeningService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public void TrackOpeningBatch(IEnumerable<CrateOpening> openings)
    {
        int count = 0;
        
        foreach (var opening in openings)
        {
            _openingsBuffer.Enqueue(opening);
            count++;
        }
        
        IncrementOpeningCounter(count);
        
        _logger.LogDebug("Enqueued {Count} openings for processing", count);
    }
    
    public async Task<long> GetTotalOpeningCountAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var counter = await dbContext.CounterStats
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == "TotalOpenings");
            
        return counter?.Value ?? 0;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Crate opening background service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_flushInterval, stoppingToken);
            await FlushBufferAsync(stoppingToken);
        }
    }

    private async Task FlushBufferAsync(CancellationToken cancellationToken)
    {
        if (_openingsBuffer.IsEmpty)
            return;
            
        var batch = new List<CrateOpening>(_batchSize);
        
        while (batch.Count < _batchSize && _openingsBuffer.TryDequeue(out var opening))
        {
            batch.Add(opening);
        }
        
        if (batch.Count > 0)
        {
            await SaveOpeningsBatchAsync(batch, cancellationToken);
            _logger.LogInformation("Processed batch of {Count} openings", batch.Count);
        }
    }

    private async Task SaveOpeningsBatchAsync(List<CrateOpening> batch, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            dbContext.CrateOpenings.AddRange(batch);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save batch of {Count} openings", batch.Count);
        }
    }

    public void IncrementOpeningCounter(int count)
    {
        _counterSemaphore.Wait();
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var counter = dbContext.CounterStats
                .FirstOrDefault(c => c.Name == "TotalOpenings");
                
            if (counter == null)
            {
                counter = new CounterStat 
                { 
                    Name = "TotalOpenings", 
                    Value = count,
                    LastUpdated = DateTime.UtcNow
                };
                dbContext.CounterStats.Add(counter);
            }
            else
            {
                counter.Value += count;
                counter.LastUpdated = DateTime.UtcNow;
            }
            
            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to increment counter by {Count}", count);
        }
        finally
        {
            _counterSemaphore.Release();
        }
    }

    public async Task InitializeCounterAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        if (!await dbContext.CounterStats.AnyAsync(c => c.Name == "TotalOpenings"))
        {
            _logger.LogInformation("Initializing counter from database count");
            var count = await dbContext.CrateOpenings.CountAsync();
            _logger.LogInformation($"Found {count} existing openings");
            
            var counter = new CounterStat
            {
                Name = "TotalOpenings",
                Value = count,
                LastUpdated = DateTime.UtcNow
            };
            
            dbContext.CounterStats.Add(counter);
            await dbContext.SaveChangesAsync();
        }
    }
}