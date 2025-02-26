using System.Net;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.Exceptions;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class PriceService
{
    private readonly ApplicationDbContext _dbContext;
    public PriceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Price>> GetAll()
    {
        var prices = await _dbContext.Prices.ToListAsync();

        if (prices == null)
        {
            throw new DomainException("No prices found", HttpStatusCode.NotFound);
        }

        return prices;
    }

    public async Task<Price> GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Id cannot be null or empty", nameof(id));
        }

        var prices = await _dbContext.Prices.FirstOrDefaultAsync(x => x.SkinId == id);

        if (prices == null)
        {
            throw new DomainException($"No price found for id {id}", HttpStatusCode.NotFound);
        }

        return prices;
    }
    
}