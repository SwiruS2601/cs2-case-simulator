using Cs2CaseOpener.DB;
using Cs2CaseOpener.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cs2CaseOpener.Converters;

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
        [property: JsonPropertyName("contains")] List<SkinDto>? contains,
        [property: JsonPropertyName("contains_rare")] List<SkinDto>? contains_rare
    );

    public record SkinDto(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string? name,
        [property: JsonPropertyName("rarity")] RarityDto? rarity,
        [property: JsonPropertyName("paint_index")] string? paint_index,
        [property: JsonPropertyName("image")] string? image
    );

    public record RarityDto(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("name")] string? name,
        [property: JsonPropertyName("color")] string? color
    );

    public record SteamPriceDto(
        [property: JsonPropertyName("last_24h")] double? last_24h,
        [property: JsonPropertyName("last_7d")] double? last_7d,
        [property: JsonPropertyName("last_30d")] double? last_30d,
        [property: JsonPropertyName("last_90d")] double? last_90d,
        [property: JsonPropertyName("last_ever")] double? last_ever
    );

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

    public async Task ScrapeApi() 
    {
        // Retrieve data from API endpoints.
        var crates = await GetCrates();
        var prices = await GetPrices();

        // Process crates and contained skins.
        foreach (var crateDto in crates)
        {
            // Check if crate exists by id.
            var crate = _dbContext.Crates.FirstOrDefault(c => c.Id == crateDto.id);
            if (crate == null)
            {
                crate = new Crate
                {
                    Id = crateDto.id,
                    Name = crateDto.name,
                    Description = crateDto.description,
                    Type = crateDto.type,
                    First_Sale_Date = crateDto.first_sale_date,
                    Market_Hash_Name = crateDto.market_hash_name,
                    Rental = crateDto.rental,
                    Image = crateDto.image,
                    Model_Player = crateDto.model_player
                };
                _dbContext.Crates.Add(crate);
            }

            var skins = new List<SkinDto>();

            if (crateDto.contains != null && crateDto.contains.Any())
            {
                skins.AddRange(crateDto.contains);
            }

            if (crateDto.contains_rare != null && crateDto.contains_rare.Any())
            {
                skins.AddRange(crateDto.contains_rare);
            }

            // If the crate contains skins, process them.
            if (skins.Any())
            {
                foreach (var skinDto in skins)
                {
                    // First check if the skin is already tracked locally.
                    var skin = _dbContext.Skins.Local.FirstOrDefault(s => s.Id == skinDto.id)
                        ?? _dbContext.Skins.FirstOrDefault(s => s.Id == skinDto.id);

                    if (skin == null)
                    {
                        if (skinDto.rarity != null)
                        {
                            var rarity = _dbContext.ChangeTracker.Entries<Rarity>()
                                .Select(e => e.Entity)
                                .FirstOrDefault(r => r.Id == skinDto.rarity.id)
                                ?? _dbContext.Rarities.FirstOrDefault(r => r.Id == skinDto.rarity.id);
                            
                            if (rarity == null)
                            {
                                rarity = new Rarity
                                {
                                    Id = skinDto.rarity.id,
                                    Name = skinDto.rarity.name,
                                    Color = skinDto.rarity.color
                                };
                                _dbContext.Rarities.Add(rarity);
                            }
                            
                            skin = new Skin
                            {
                                Id = skinDto.id,
                                Name = skinDto.name,
                                RarityId = skinDto.rarity.id,
                                PaintIndex = skinDto.paint_index,
                                Image = skinDto.image
                            };
                        }
                        else
                        {
                            skin = new Skin
                            {
                                Id = skinDto.id,
                                Name = skinDto.name,
                                PaintIndex = skinDto.paint_index,
                                Image = skinDto.image
                            };
                        }
                        _dbContext.Skins.Add(skin);
                    }

                    // Establish the relationship between crate and skin if not already linked.
                    if (crate.Skins == null)
                    {
                        crate.Skins = new List<Skin>();
                    }

                    if (!crate.Skins.Any(s => s.Id == skin.Id))
                    {
                        crate.Skins.Add(skin);
                    }
                }
            }
        }

        // Process price entries.
        foreach (var kvp in prices)
        {
            // Expected key format: "Skin Name (Wear)"
            var key = kvp.Key;
            var idxOpen = key.LastIndexOf('(');
            var idxClose = key.LastIndexOf(')');
            if (idxOpen > 0 && idxClose > idxOpen)
            {
                var skinName = key.Substring(0, idxOpen).Trim();
                var wear = key.Substring(idxOpen + 1, idxClose - idxOpen - 1).Trim();

                // Find the skin by Name (assuming unique names here)
                var skin = _dbContext.Skins.FirstOrDefault(s => s.Name == skinName);
                if (skin != null)
                {
                    // Try to find an existing price record.
                    var existingPrice = _dbContext.Prices.FirstOrDefault(p => p.SkinId == skin.Id && p.Wear_Category == wear);
                    if (existingPrice != null)
                    {
                        // Update the fields.
                        existingPrice.Steam_Last_24h = kvp.Value.steam.last_24h;
                        existingPrice.Steam_Last_7d = kvp.Value.steam.last_7d;
                        existingPrice.Steam_Last_30d = kvp.Value.steam.last_30d;
                        existingPrice.Steam_Last_90d = kvp.Value.steam.last_90d;
                        existingPrice.Steam_Last_Ever = kvp.Value.steam.last_ever;
                    }
                    else
                    {
                        // Create and add a new price.
                        var price = new Price
                        {
                            SkinId = skin.Id,
                            Wear_Category = wear,
                            Steam_Last_24h = kvp.Value.steam.last_24h,
                            Steam_Last_7d = kvp.Value.steam.last_7d,
                            Steam_Last_30d = kvp.Value.steam.last_30d,
                            Steam_Last_90d = kvp.Value.steam.last_90d,
                            Steam_Last_Ever = kvp.Value.steam.last_ever
                        };
                        _dbContext.Prices.Add(price);
                    }
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}