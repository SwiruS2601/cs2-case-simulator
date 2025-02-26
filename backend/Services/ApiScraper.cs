using Cs2CaseOpener.DB;
using Cs2CaseOpener.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cs2CaseOpener.Converters;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Npgsql;

namespace Cs2CaseOpener.Services;
public class ApiScraper
{
    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public ApiScraper(ApplicationDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }
    
    public record CrateDto(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string? name,
        [property: JsonPropertyName("description")] string? description,
        [property: JsonPropertyName("type")] string? type,
        [property: JsonPropertyName("first_sale_date")] DateTime? first_sale_date,
        [property: JsonPropertyName("market_hash_name")] string? market_hash_name,
        [property: JsonPropertyName("rental")] bool? rental,
        [property: JsonPropertyName("image")] string? image,
        [property: JsonPropertyName("model_player")] string? model_player,
        [property: JsonPropertyName("contains")] List<ItemDto>? contains,
        [property: JsonPropertyName("contains_rare")] List<ItemDto>? contains_rare
    );

    public record ItemDto(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string? name,
        [property: JsonPropertyName("rarity")] RarityDto? rarity,
        [property: JsonPropertyName("paint_index")] string? paint_index,
        [property: JsonPropertyName("image")] string? image,
        [property: JsonPropertyName("stattrak")] bool? stattrak,
        [property: JsonPropertyName("souvenir")] bool? souvenir,
        [property: JsonPropertyName("min_float")] string? min_float,
        [property: JsonPropertyName("max_float")] string? max_float,
        [property: JsonPropertyName("category")] IdNameDto? category,
        [property: JsonPropertyName("pattern")] IdNameDto? pattern
    );

    public record IdNameDto(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string name
    );

    public record RarityDto(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("color")] string color
    );

    public record SteamPriceDto(
        [property: JsonPropertyName("last_24h")] double? last_24h,
        [property: JsonPropertyName("last_7d")] double? last_7d,
        [property: JsonPropertyName("last_30d")] double? last_30d,
        [property: JsonPropertyName("last_90d")] double? last_90d,
        [property: JsonPropertyName("last_ever")] double? last_ever
    );

    private readonly string[] validWears = ["Minimal Wear", "Field-Tested", "Battle-Scarred", "Well-Worn", "Factory New"];
    private readonly string[] allowedTypes = ["Case", "Souvenir", "Sticker Capsule", "Autograph Capsule"];


    public record PriceDetailDto(
        [property: JsonPropertyName("steam")] SteamPriceDto steam
    );

    public async Task<List<CrateDto>> GetCrates()
    {
        var res = await _httpClient.GetAsync("https://bymykel.github.io/CSGO-API/api/en/crates.json");
        var content = await res.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content)) throw new Exception("Failed to fetch crates");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new CustomDateTimeConverter());
        var crates = JsonSerializer.Deserialize<List<CrateDto>>(content, options);
        if (crates == null) throw new Exception("Failed to parse crates");

        return crates;
    }

    public async Task<Dictionary<string, PriceDetailDto>> GetPrices()
    {
        var res = await _httpClient.GetAsync("https://raw.githubusercontent.com/ByMykel/counter-strike-price-tracker/main/static/prices/latest.json");
        var content = await res.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content)) throw new Exception("Failed to fetch prices");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var prices = JsonSerializer.Deserialize<Dictionary<string, PriceDetailDto>>(content, options);
        if (prices == null) throw new Exception("Failed to parse prices");

        return prices;
    }

    private static string NormalizeAncientName(string name)
    {
        return name.Replace("Sticker | ", "")
            .Replace("Autograph | ", "")
            .Replace("Ancient ", "")
            .Trim();
    }

    private static string NormalizePriceName(string name)
    {
        return name.Replace("Sticker | ", "")
            .Replace("Autograph | ", "")
            .Trim();
    }

    public async Task ScrapeApi() 
    {
        var crates = await GetCrates();
        var prices = await GetPrices();

        _dbContext.ChangeTracker.Clear();
        var existingRarities = await _dbContext.Rarities.ToDictionaryAsync(r => r.Id);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var uniqueRarities = crates
                .SelectMany(c => c.contains?.Select(s => s.rarity) ?? Enumerable.Empty<RarityDto>())
                .Concat(crates.SelectMany(c => c.contains_rare?.Select(s => s.rarity) ?? Enumerable.Empty<RarityDto>()))
                .Where(r => r != null)
                .DistinctBy(r => r!.id)
                .ToList();

            foreach (var rarityDto in uniqueRarities)
            {
                if (!existingRarities.ContainsKey(rarityDto!.id))
                {
                    var rarity = new Rarity
                    {
                        Id = rarityDto!.id,
                        Name = rarityDto.name,
                        Color = rarityDto.color
                    };
                    _dbContext.Rarities.Add(rarity);
                    existingRarities[rarity.Id] = rarity;
                }
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            var savedCount = await _dbContext.Rarities.CountAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Failed to process rarities: {ex.Message}", ex);
        }

        await using var mainTransaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _dbContext.ChangeTracker.Clear();
            
            foreach (var crateDto in crates)
            {
                if (!allowedTypes.Contains(crateDto.type ?? string.Empty))
                {
                    continue; 
                }
                
                var crate = await _dbContext.Crates.Include(c => c.Skins)
                    .FirstOrDefaultAsync(c => c.Id == crateDto.id);
                    
                if (crate == null)
                {
                    crate = new Crate
                    {
                        Id = crateDto.id,
                        Name = crateDto.name,
                        Description = crateDto.description,
                        Type = crateDto.type,
                        Market_Hash_Name = crateDto.market_hash_name,
                        Rental = crateDto.rental,
                        Image = crateDto.image,
                        Model_Player = crateDto.model_player
                    };
                    crate.SetFirstSaleDate(crateDto.first_sale_date);
                    _dbContext.Crates.Add(crate);
                }

                var items = new List<ItemDto>();
                if (crateDto.contains != null) items.AddRange(crateDto.contains);
                if (crateDto.contains_rare != null) items.AddRange(crateDto.contains_rare);

                foreach (var ItemDto in items.Where(s => s != null))
                {
                    var item = await _dbContext.Skins
                        .FirstOrDefaultAsync(s => s.Id == ItemDto.id);

                    if (item == null)
                    {
                        item = new Skin
                        {
                            Id = ItemDto.id,
                            Name = ItemDto.name,
                            RarityId = ItemDto.rarity?.id,
                            PaintIndex = ItemDto.paint_index,
                            Image = ItemDto.image,
                            MinFloat = double.TryParse(ItemDto.min_float, out var minFloat) ? minFloat : (double?)null,
                            MaxFloat = double.TryParse(ItemDto.max_float, out var maxFloat) ? maxFloat : (double?)null,
                            Souvenir = ItemDto.souvenir,
                            StatTrak = ItemDto.stattrak,
                            Category = ItemDto.category?.id,
                            Pattern = ItemDto.pattern?.id
                        };

                        if (ItemDto.rarity != null)
                        {
                            if (!existingRarities.TryGetValue(ItemDto.rarity.id, out var rarity))
                            {
                                rarity = new Rarity
                                {
                                    Id = ItemDto.rarity.id,
                                    Name = ItemDto.rarity.name,
                                    Color = ItemDto.rarity.color
                                };
                                existingRarities[rarity.Id] = rarity;
                                _dbContext.Rarities.Add(rarity);
                            }
                        }

                        _dbContext.Skins.Add(item);
                    }

                    if (!crate.Skins.Any(s => s.Id == item.Id))
                    {
                        crate.Skins.Add(item);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            const int batchSize = 100;
            var currentBatch = new List<Price>();
            int exactMatches = 0;
            int substringMatches = 0;
            int noMatches = 0;

            var processedCombinations = new HashSet<(string SkinId, string Wear)>();

            var allSkins = await _dbContext.Skins.ToListAsync();
            
            var existingPrices = await _dbContext.Prices
                .Select(p => new { p.SkinId, p.Wear_Category })
                .ToListAsync();
                
            foreach (var existing in existingPrices)
            {
                processedCombinations.Add((existing.SkinId, existing.Wear_Category));
            }

            foreach (var kvp in prices)
            {
                var key = kvp.Key;

                // If the key indicates a sticker or autograph, use normalization.
                if (key.Contains("Sticker", StringComparison.OrdinalIgnoreCase) ||
                    key.Contains("Autograph", StringComparison.OrdinalIgnoreCase))
                {
                    key = NormalizePriceName(key);
                }

                string wear = validWears.FirstOrDefault(x => key.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0) ?? "Default";
                string skinName = (wear != "Default")
                    ? (key.IndexOf('(') >= 0 ? key.Substring(0, key.IndexOf('(')).Trim() : key.Replace(wear, "", StringComparison.OrdinalIgnoreCase).Trim())
                    : key.Trim();
                
                // Try exact then substring matching
                var skin = allSkins.FirstOrDefault(s => string.Equals(s.Name, skinName, StringComparison.OrdinalIgnoreCase))
                         ?? allSkins.FirstOrDefault(s => s.Name != null && (s.Name.Contains(skinName, StringComparison.OrdinalIgnoreCase) || 
                                                                           skinName.Contains(s.Name, StringComparison.OrdinalIgnoreCase)));
                string matchType = skin == null ? "none" : (string.Equals(skin?.Name, skinName, StringComparison.OrdinalIgnoreCase) ? "exact" : "substring");

                // If skin found and its rarity is ancient, try normalized matching if initial match seems off
                if (skin != null && skin.RarityId.Equals("rarity_ancient", StringComparison.OrdinalIgnoreCase))
                {
                    // Compare normalized names
                    if (!string.Equals(NormalizeAncientName(skin.Name), NormalizeAncientName(skinName), StringComparison.OrdinalIgnoreCase))
                    {
                        // Attempt to find an alternative match using normalized names
                        var altSkin = allSkins.FirstOrDefault(s => s.Name != null && 
                                            string.Equals(NormalizeAncientName(s.Name), NormalizeAncientName(skinName), StringComparison.OrdinalIgnoreCase));
                        if (altSkin != null)
                        {
                            skin = altSkin;
                            matchType = "ancient-normalized";
                        }
                    }
                }
                
                if (skin != null)
                {
                    // Check for duplicates in tracking sets (unchanged)
                    if (processedCombinations.Contains((skin.Id, wear)) ||
                        currentBatch.Any(p => p.SkinId == skin.Id && p.Wear_Category == wear))
                    {
                        continue;
                    }
                    
                    processedCombinations.Add((skin.Id, wear));
                    
                    if (matchType == "exact") exactMatches++;
                    else substringMatches++;
                    
                    var existingPrice = await _dbContext.Prices.FirstOrDefaultAsync(p => p.SkinId == skin.Id && p.Wear_Category == wear);
                    if (existingPrice != null)
                    {
                        existingPrice.Steam_Last_24h = kvp.Value.steam.last_24h;
                        existingPrice.Steam_Last_7d = kvp.Value.steam.last_7d;
                        existingPrice.Steam_Last_30d = kvp.Value.steam.last_30d;
                        existingPrice.Steam_Last_90d = kvp.Value.steam.last_90d;
                        existingPrice.Steam_Last_Ever = kvp.Value.steam.last_ever;
                        currentBatch.Add(existingPrice);
                    }
                    else
                    {
                        var price = new Price
                        {
                            SkinId = skin.Id,
                            Wear_Category = wear,
                            Name = key,
                            Steam_Last_24h = kvp.Value.steam.last_24h,
                            Steam_Last_7d = kvp.Value.steam.last_7d,
                            Steam_Last_30d = kvp.Value.steam.last_30d,
                            Steam_Last_90d = kvp.Value.steam.last_90d,
                            Steam_Last_Ever = kvp.Value.steam.last_ever
                        };
                        _dbContext.Prices.Add(price);
                        currentBatch.Add(price);
                    }
                    
                    if (currentBatch.Count >= batchSize)
                    {
                        try
                        {
                            await _dbContext.SaveChangesAsync();
                            _dbContext.ChangeTracker.Clear();
                            currentBatch.Clear();
                        }
                        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                        {
                            _dbContext.ChangeTracker.Clear();
                            currentBatch.Clear();
                        }
                    }
                }
                else
                {
                    noMatches++;
                }
            }

            foreach (var kvp in prices)
            {
                var key = kvp.Key;
                var crate = await _dbContext.Crates.FirstOrDefaultAsync(c => c.Name == key);
                if (crate == null)
                {
                    continue;
                }
                
                var cratePrice = await _dbContext.Prices.FirstOrDefaultAsync(p => p.CrateId == crate.Id);
                if (cratePrice != null)
                {
                    cratePrice.Steam_Last_24h = kvp.Value.steam.last_24h;
                    cratePrice.Steam_Last_7d = kvp.Value.steam.last_7d;
                    cratePrice.Steam_Last_30d = kvp.Value.steam.last_30d;
                    cratePrice.Steam_Last_90d = kvp.Value.steam.last_90d;
                    cratePrice.Steam_Last_Ever = kvp.Value.steam.last_ever;
                }
                else
                {
                    var newPrice = new Price
                    {
                        CrateId = crate.Id,
                        Name = crate.Name,
                        Steam_Last_24h = kvp.Value.steam.last_24h,
                        Steam_Last_7d = kvp.Value.steam.last_7d,
                        Steam_Last_30d = kvp.Value.steam.last_30d,
                        Steam_Last_90d = kvp.Value.steam.last_90d,
                        Steam_Last_Ever = kvp.Value.steam.last_ever,
                    };
                    _dbContext.Prices.Add(newPrice);
                }
            }
            
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"Price matching summary - Exact: {exactMatches}, Substring: {substringMatches}, Not Found: {noMatches}");

            if (currentBatch.Any())
            {
                await _dbContext.SaveChangesAsync();
            }

            await mainTransaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await mainTransaction.RollbackAsync();
            throw new Exception($"Failed to process main data: {ex.Message}", ex);
        }

        var finalRaritiesCount = await _dbContext.Rarities.CountAsync();
        var finalPricesCount = await _dbContext.Prices.CountAsync();
        Console.WriteLine($"Final counts - Rarities: {finalRaritiesCount}, Prices: {finalPricesCount}");
    }
}