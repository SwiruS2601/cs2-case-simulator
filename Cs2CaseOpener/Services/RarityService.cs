using System.Net;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Exceptions;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class RarityService
{
    private readonly ApplicationDbContext _dbContext;

    public RarityService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Rarity>> GetAll()
    {
        var rarities = await _dbContext.Rarities.ToListAsync();

        if  (rarities == null)
        {
            throw new DomainException("No rarities found", HttpStatusCode.NotFound);
        }

        return rarities;
    }
}