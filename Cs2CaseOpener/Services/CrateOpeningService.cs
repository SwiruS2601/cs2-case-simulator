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
    private static DateTime _lastCountUpdate = DateTime.MinValue;
    private static int _cachedTotalCount = 0;
    private static readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

    public CrateOpeningService(
        IServiceScopeFactory scopeFactory,
        ILogger<CrateOpeningService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<int> GetTotalOpeningCountAsync()
    {
        if (DateTime.UtcNow - _lastCountUpdate > TimeSpan.FromSeconds(30))
        {
            await _cacheLock.WaitAsync();
            try
            {
                if (DateTime.UtcNow - _lastCountUpdate > TimeSpan.FromSeconds(30))
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        _cachedTotalCount = await dbContext.CrateOpenings.CountAsync();
                        _lastCountUpdate = DateTime.UtcNow;
                    }
                }
            }
            finally
            {
                _cacheLock.Release();
            }
        }
        
        return _cachedTotalCount;
    }

    public void TrackOpening(CrateOpening opening)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.CrateOpenings.Add(opening);
            dbContext.SaveChanges();
            
            Interlocked.Increment(ref _cachedTotalCount);
        }
    }

    public void TrackOpenings(IEnumerable<CrateOpening> openings)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.CrateOpenings.AddRange(openings);
            dbContext.SaveChanges();
            
            Interlocked.Add(ref _cachedTotalCount, openings.Count());
        }
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save crate openings batch: {Message}", ex.Message);
            dbContext.ChangeTracker.Clear();
            throw;
        }
    }

}