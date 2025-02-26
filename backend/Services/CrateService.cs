using System.Net;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.Exceptions;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cs2CaseOpener.Services;

public class CrateService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    private static readonly string CrateCacheKey = "CrateService_GetAll";

    private static readonly string CrateRelevantCacheKey = "CrateService_GetRelevantCrates";

    private static readonly string[] AllowedTypes = ["Case", "Souvenir", "Sticker Capsule", "Autograph Capsule"];

    private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public CrateService(ApplicationDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }
    
    private IQueryable<Crate> GetCrateQuery() =>
        _dbContext.Crates.AsNoTracking()
            .Include(c => c.Price)
            .Include(c => c.Skins)
                .ThenInclude(s => s.Rarity)
            .Include(c => c.Skins)
                .ThenInclude(s => s.Prices);

    public async Task<List<Crate>> GetAll()
    {
        
        var cached = _cache.Get<List<Crate>>(CrateCacheKey);
        if (cached != null)
        {
            return cached;
        }

        var crates = await _dbContext.Crates
            .AsNoTracking()
            .Include(c => c.Price)
            .ToListAsync();
            
        if (crates is null || crates.Count == 0)
        {
            throw new DomainException("No crates found", HttpStatusCode.NotFound);
        }

        _cache.Set(CrateCacheKey, crates, _cacheOptions);
  
        return crates;
    }

    public async Task<List<Crate>> GetAllRelevantCrates()
    {

        var cached = _cache.Get<List<Crate>>(CrateRelevantCacheKey);
        if (cached != null)
        {
            return cached;
        }

        var crates = await _dbContext.Crates.AsNoTracking()
            .Where(c => AllowedTypes.Contains(c.Type))
            .Include(c => c.Price)
            .ToListAsync();

        if (crates is null || crates.Count == 0)
        {
            throw new DomainException("No crates found", HttpStatusCode.NotFound);
        }

        _cache.Set(CrateRelevantCacheKey, crates, _cacheOptions);
        return crates;
    }

    public async Task<Crate> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Id cannot be null or empty", nameof(id));
        }

        string cacheKey = $"Crate_{id}";

        var cached = _cache.Get<Crate>(cacheKey);
        if (cached != null)
        {
            return cached;
        }
        
        var crate = await GetCrateQuery().FirstOrDefaultAsync(c => c.Id == id) 
            ?? throw new DomainException($"No crate found for id {id}", HttpStatusCode.NotFound);

        _cache.Set(cacheKey, crate, _cacheOptions);

        return crate;
    }

    public async Task<Crate> GetByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }

        string cacheKey = $"Crate_Name_{name}";
        var cached = _cache.Get<Crate>(cacheKey);

        if (cached != null)
        {
            return cached;
        }

        var crate = await GetCrateQuery().FirstOrDefaultAsync(c => c.Name == name)
            ?? throw new DomainException($"No crate found for name {name}", HttpStatusCode.NotFound);

        return crate;
    }
}