using System.Net;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.Exceptions;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class CrateService
{
    private readonly ApplicationDbContext _dbContext;

    public CrateService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Crate>> GetAll()
    {
        var crates = await _dbContext.Crates.ToListAsync();

        if (crates == null)
        {
            throw new DomainException("No crates found", HttpStatusCode.NotFound);
        }

        return crates;
    }

    public async Task<Crate> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Id cannot be null or empty", nameof(id));
        }

        var crate = await _dbContext.Crates
            .Include(c => c.Skins)
                .ThenInclude(s => s.Rarity)
            .Include(c => c.Skins)
                .ThenInclude(s => s.Prices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (crate == null)
        {
            throw new DomainException($"No crate found for id {id}", HttpStatusCode.NotFound);
        }

        return crate;
    }

    public async Task<Crate> GetByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }

        var crate = await _dbContext.Crates
            .Include(c => c.Skins)
                .ThenInclude(s => s.Rarity)
            .Include(c => c.Skins)
                .ThenInclude(s => s.Prices)
            .FirstOrDefaultAsync(c => c.Name == name);

        if (crate == null)
        {
            throw new DomainException($"No crate found for name {name}", HttpStatusCode.NotFound);
        }

        return crate;
    }
}