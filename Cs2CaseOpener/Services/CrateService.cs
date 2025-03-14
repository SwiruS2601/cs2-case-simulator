using System.Net;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Exceptions;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class CrateService
{
    private readonly ApplicationDbContext _dbContext;
    private static readonly string[] AllowedTypes = ["Case", "Souvenir", "Sticker Capsule", "Autograph Capsule"];

    public CrateService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Crate>> GetAll()
    {
        var crates = await _dbContext.Crates
            .AsNoTracking()
            .Include(c => c.Price)
            .ToListAsync();
            
        if (crates is null || crates.Count == 0)
            throw new DomainException("No crates found", HttpStatusCode.NotFound);

        return crates;
    }

    public async Task<List<Crate>> GetAllRelevantCrates()
    {
        var crates = await _dbContext.Crates.AsNoTracking()
            .Where(c => AllowedTypes.Contains(c.Type))
            .Include(c => c.Price)
            .ToListAsync();

        if (crates is null || crates.Count == 0)
            throw new DomainException("No crates found", HttpStatusCode.NotFound);

        return crates;
    }

    public async Task<List<Crate>> GetCases()
    {
        var crates = await _dbContext.Crates.AsNoTracking()
            .Where(c => c.Type == "Case")
            .Include(c => c.Price)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        if (crates is null || crates.Count == 0)
            throw new DomainException("No cases found", HttpStatusCode.NotFound);

        return crates;
    }

    public async Task<List<Crate>> GetSouvenirs()
    {
        var crates = await _dbContext.Crates.AsNoTracking()
            .Where(c => c.Type == "Souvenir")
            .Include(c => c.Price)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        if (crates is null || crates.Count == 0)
            throw new DomainException("No souvenirs found", HttpStatusCode.NotFound);

        return crates;
    }

    public async Task<List<Crate>> GetStickerCapsules()
    {
        var crates = await _dbContext.Crates.AsNoTracking()
            .Where(c => c.Type == "Sticker Capsule")
            .Include(c => c.Price)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        if (crates is null || crates.Count == 0)
            throw new DomainException("No sticker capsules found", HttpStatusCode.NotFound);

        return crates;
    }

    public async Task<List<Crate>> GetAutographCapsules()
    {
        var crates = await _dbContext.Crates.AsNoTracking()
            .Where(c => c.Type == "Autograph Capsule")
            .Include(c => c.Price)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        if (crates is null || crates.Count == 0)
            throw new DomainException("No autograph capsules found", HttpStatusCode.NotFound);

        return crates;
    }

    private IQueryable<Crate> GetCrateQuery() =>
        _dbContext.Crates.AsNoTracking()
            .Include(c => c.Price)
            .Include(c => c.Skins)
                .ThenInclude(s => s.Rarity)
            .Include(c => c.Skins)
                .ThenInclude(s => s.Prices);

    public async Task<Crate> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Id cannot be null or empty", nameof(id));
        }

        string cacheKey = $"Crate_{id}";
        
        var crate = await GetCrateQuery().FirstOrDefaultAsync(c => c.Id == id) 
            ?? throw new DomainException($"No crate found for id {id}", HttpStatusCode.NotFound);

        return crate;
    }

    public async Task<Crate> GetByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }

        var crate = await GetCrateQuery().FirstOrDefaultAsync(c => c.Name == name)
            ?? throw new DomainException($"No crate found for name {name}", HttpStatusCode.NotFound);

        return crate;
    }

    public async Task<Crate[]> Search(string searchKey)
    {
        if (string.IsNullOrEmpty(searchKey))
        {
            throw new ArgumentException("Search key cannot be null or empty", nameof(searchKey));
        }

        var crates = await _dbContext.Crates
            .AsNoTracking()
            .Where(c => AllowedTypes.Contains(c.Type))
            .Where(c => c.Name!.ToLower().Contains(searchKey.ToLower()))
            .Take(20)
            .ToArrayAsync();

        if (crates is null || crates.Length == 0)
        {
            return [];
        }

        return crates;
    }
}