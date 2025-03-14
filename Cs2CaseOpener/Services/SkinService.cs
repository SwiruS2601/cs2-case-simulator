using System.Net;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Exceptions;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class SkinService
{
    private readonly ApplicationDbContext _dbContext;

    public SkinService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Skin>> GetAll()
    {
        var skins = await _dbContext.Skins.ToListAsync();

        if  (skins == null)
        {
            throw new DomainException("No skins found", HttpStatusCode.NotFound);
        }

        return skins;
    }

    public async Task<Skin> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Id cannot be null or empty", nameof(id));
        }

        var skins =  await _dbContext.Skins.Include(s => s.Rarity)
            .Include(s => s.Prices)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (skins == null)
        {
            throw new DomainException($"No skin found for id {id}", HttpStatusCode.NotFound);
        }
        
        return skins;
    }
   
}
