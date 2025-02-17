using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.DTOs;
using Cs2CaseOpener.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class CaseService
{
    private readonly ApplicationDbContext _dbContext;

    public CaseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CaseResponseDTO>> GetCases()
    {
        var cases = await _dbContext.Cases
            .AsNoTracking()
            .Select(c => new CaseResponseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                FirstSaleDate = c.FirstSaleDate,
                MarketHashName = c.MarketHashName,
                Rental = c.Rental,
                Image = c.Image,
                ModelPlayer = c.ModelPlayer
            })
            .ToListAsync();
            

        if (cases == null || cases.Count == 0)
        {
            throw new DomainException("An error occurred while fetching cases", HttpStatusCode.NotFound);
        }

        return cases;
    }

    public async Task<CaseResponseDTO?> GetCaseByIdAsync(string id)
    {
        var case_ = await _dbContext.Cases
            .AsNoTracking()
            .Include(c => c.Skins)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (case_ == null)
        {
            throw new DomainException($"No case found for id {id}", HttpStatusCode.NotFound);
        }

        return new CaseResponseDTO
        {
            Id = case_.Id,
            Name = case_.Name,
            Type = case_.Type,
            FirstSaleDate = case_.FirstSaleDate,
            MarketHashName = case_.MarketHashName,
            Rental = case_.Rental,
            Image = case_.Image,
            ModelPlayer = case_.ModelPlayer,
            Skins = case_.Skins?.Select(s => new CaseSkinDto
            {
                Id = s.Id,
                Name = s.Name,
                Type = s.Type,
                WeaponType = s.WeaponType,
                GunType = s.GunType,
                Rarity = s.Rarity,
                RarityColor = s.RarityColor,
                Prices = s.Prices != null ? JsonSerializer.Deserialize<JsonObject>(s.Prices) : null,
                FirstSaleDate = s.FirstSaleDate,
                KnifeType = s.KnifeType,
                Image = s.Image,
                MinFloat = s.MinFloat,
                MaxFloat = s.MaxFloat,
                Stattrak = s.Stattrak
            }).ToList()
        };
    }

}