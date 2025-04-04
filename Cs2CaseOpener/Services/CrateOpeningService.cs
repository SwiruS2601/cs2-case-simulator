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
        
        var validOpenings = new List<CrateOpening>(batch.Capacity);
        foreach (var opening in batch)
        {
            // Determine if the item is likely a sticker based on SkinId prefix
            bool isSticker = opening.SkinId?.StartsWith("sticker-") ?? false;

            // Validation: Always require SkinName. Require PaintIndex only if NOT a sticker.
            if (string.IsNullOrEmpty(opening.SkinName) || (!isSticker && opening.PaintIndex == null))
            {
                _logger.LogWarning("Skipping opening record (Original SkinId: {OriginalSkinId}, IsSticker: {IsSticker}) because required info (SkinName or PaintIndex if not sticker) is missing.", 
                    opening.SkinId, isSticker);
                continue; 
            }
            
            // Look up the CURRENT Skin record conditionally
            Skin? currentSkin;
            if (isSticker)
            {
                // Look up stickers by Name only
                currentSkin = await dbContext.Skins
                    .AsNoTracking() 
                    .FirstOrDefaultAsync(s => s.Name == opening.SkinName, cancellationToken);
            }
            else
            {
                // Look up non-stickers by Name AND PaintIndex
                string? paintIndexString = opening.PaintIndex?.ToString(); 
                currentSkin = await dbContext.Skins
                    .AsNoTracking() 
                    .FirstOrDefaultAsync(s => 
                        s.Name == opening.SkinName &&
                        s.PaintIndex == paintIndexString, 
                        cancellationToken);
            }

            if (currentSkin != null)
            {
                // Found the definitive skin. Update the SkinId if needed.
                if (opening.SkinId != currentSkin.Id)
                {
                    _logger.LogInformation("Correcting SkinId for opening. Original: {OriginalId}, Corrected: {CorrectedId} (Name: {Name}, IsSticker: {IsSticker})", 
                        opening.SkinId, currentSkin.Id, opening.SkinName, isSticker);
                    opening.SkinId = currentSkin.Id;
                }
                validOpenings.Add(opening); 
            }
            else
            {
                 // Log slightly differently depending on sticker status
                if (isSticker) {
                     _logger.LogError("Failed to find definitive Sticker record for Name: {Name} before saving CrateOpening. Original SkinId was {OriginalSkinId}. Skipping this opening.", 
                        opening.SkinName, opening.SkinId);
                }
                 else {
                    _logger.LogError("Failed to find definitive Skin record for Name: {Name}, PaintIndex: {Index} before saving CrateOpening. Original SkinId was {OriginalSkinId}. Skipping this opening.", 
                        opening.SkinName, opening.PaintIndex, opening.SkinId);
                }
            }
        }

        if (!validOpenings.Any())
        {
            _logger.LogWarning("No valid openings to save in this batch after validation.");
            return;
        }

        try
        {
            dbContext.CrateOpenings.AddRange(validOpenings);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23503")
        {
            _logger.LogError(pgEx, "Foreign key violation saving batch of {Count} openings despite pre-check. SqlState: {SqlState}. Check data consistency.", 
                validOpenings.Count, pgEx.SqlState);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save batch of {Count} openings", validOpenings.Count);
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