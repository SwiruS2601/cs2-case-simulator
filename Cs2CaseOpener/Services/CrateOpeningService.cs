using System.Collections.Concurrent;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Interfaces;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class CrateOpeningService : BackgroundService
{
    private readonly ConcurrentQueue<CrateOpening> _openingsBuffer = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CrateOpeningService> _logger;
    private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(10);
    private readonly int _batchSize = 100;
    private static readonly SemaphoreSlim _counterSemaphore = new(1, 1);

    public CrateOpeningService(
        IServiceScopeFactory scopeFactory,
        ILogger<CrateOpeningService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
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

    public void TrackOpening(CrateOpening opening)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.CrateOpenings.Add(opening);
            dbContext.SaveChanges();
            
            IncrementOpeningCounter(1);
        }
    }

    public void TrackOpenings(IEnumerable<CrateOpening> openings)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var count = 0;
            
            dbContext.CrateOpenings.AddRange(openings);
            dbContext.SaveChanges();
            
            foreach (var _ in openings)
            {
                count++;
            }
            
            IncrementOpeningCounter(count);
        }
    }

    public void TrackOpeningBatch(IEnumerable<CrateOpening> openings)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = 0;
        
        dbContext.CrateOpenings.AddRange(openings);
        dbContext.SaveChanges();
        
        foreach (var _ in openings)
        {
            count++;
        }
        
        IncrementOpeningCounter(count);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessBuffer(stoppingToken);
            await Task.Delay(_flushInterval, stoppingToken);
        }
        
        await ProcessBuffer(stoppingToken);
    }

    private async Task ProcessBuffer(CancellationToken cancellationToken)
    {
        if (_openingsBuffer.IsEmpty)
            return;

        var batch = new List<CrateOpening>();
        while (batch.Count < _batchSize && _openingsBuffer.TryDequeue(out var opening))
        {
            batch.Add(opening);
        }

        if (batch.Count == 0)
            return;

        try
        {
            await SaveOpeningsBatchAsync(batch, cancellationToken);
            _logger.LogInformation("Processed {Count} crate openings", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving crate openings batch: {Message}", ex.Message);
        }
    }

    private async Task SaveOpeningsBatchAsync(List<CrateOpening> batch, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            dbContext.CrateOpenings.AddRange(batch);
            await unitOfWork.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
            
            await IncrementOpeningCounterAsync(batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save crate openings batch: {Message}", ex.Message);
            dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task IncrementOpeningCounterAsync(int count)
    {
        await _counterSemaphore.WaitAsync();
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var counter = await dbContext.CounterStats
                .FirstOrDefaultAsync(c => c.Name == "TotalOpenings");
                
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
            
            await dbContext.SaveChangesAsync();
        }
        finally
        {
            _counterSemaphore.Release();
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