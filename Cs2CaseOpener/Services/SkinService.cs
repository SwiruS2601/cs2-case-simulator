using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.Models;
using Microsoft.Extensions.Caching.Memory;
using Cs2CaseOpener.DTOs;
using System.Text.Json.Nodes;
using Cs2CaseOpener.Exceptions;
using System.Net;

namespace Cs2CaseOpener.Services;

public class SkinService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<Skin>> GetAllSkinsQuery = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Skins
                .AsNoTracking()
                .Include(s => s.Cases));

    public SkinService(ApplicationDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<Skin?> GetSkinByIdAsync(string id)
    {
        var skin = await _dbContext.Skins.FindAsync(id);

        if (skin == null)
        {
            throw new DomainException($"No skin found for id {id}", HttpStatusCode.NotFound);
        }

        return skin;
    }
    
    public async Task<IEnumerable<SkinDTO>> GetAllSkinsAsync()
    {

        const string cacheKey = "AllSkins";
        
        if (_cache.TryGetValue(cacheKey, out List<SkinDTO>? cachedSkins) && cachedSkins is not null)
        {
            return cachedSkins;
        }        
        
        var skins = new List<SkinDTO>();
        await foreach (var skin in GetAllSkinsQuery(_dbContext))
        {
            var skinDto = new SkinDTO
            {
                Id = skin.Id,
                Name = skin.Name,
                Classid = skin.Classid,
                Type = skin.Type,
                WeaponType = skin.WeaponType,
                GunType = skin.GunType,
                Rarity = skin.Rarity,
                RarityColor = skin.RarityColor,
                FirstSaleDate = skin.FirstSaleDate,
                KnifeType = skin.KnifeType,
                Image = skin.Image,
                MinFloat = skin.MinFloat.HasValue ? (float)skin.MinFloat.Value : null,
                MaxFloat = skin.MaxFloat.HasValue ? (float)skin.MaxFloat.Value : null,
                Stattrak = skin.Stattrak,
                Cases = skin.Cases.Select(c => new CaseInfo 
                { 
                    Id = c.Id, 
                    Name = c.Name 
                }).ToList(),
                Prices = null
            };

            if (skin.Prices != null)
            {
                try
                {
                    skinDto.Prices = JsonSerializer.Deserialize<JsonObject>(skin.Prices, _jsonOptions);
                }
                catch
                {
                    skinDto.Prices = null;
                }
            }

            skins.Add(skinDto);
        }

        if(!skins.Any())
        {
            throw new DomainException("An error occurred while fetching skins", HttpStatusCode.NotFound);
        }

        _cache.Set(cacheKey, skins, CacheDuration);
        return skins;
    }
}
