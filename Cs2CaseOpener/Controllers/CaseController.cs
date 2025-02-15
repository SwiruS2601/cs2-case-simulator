using System.Text.Json;
using System.Text.Json.Nodes;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/case")]
public class CaseController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CaseController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetCases()
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

        return Ok(cases);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCaseById(string id)
    {
        var case_ = await _dbContext.Cases
            .AsNoTracking()
            .Include(c => c.Skins)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (case_ == null)
        {
            return NotFound();
        }

        var response = new CaseResponseDTO
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

        return Ok(response);
    }
}
