using Cs2CaseOpener.Contracts;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Interfaces;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cs2CaseOpener.Services;

public class ApiScraper : IApiScraper
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IByMykelDataService _dataService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApiScraper> _logger;
    private readonly string[] ValidWears = ["Minimal Wear", "Field-Tested", "Battle-Scarred", "Well-Worn", "Factory New"];
    private readonly string[] AllowedTypes = ["Case", "Souvenir", "Sticker Capsule", "Autograph Capsule"];
    private const int BatchSize = 100;

    public ApiScraper(ApplicationDbContext dbContext, IByMykelDataService dataService, IUnitOfWork unitOfWork, ILogger<ApiScraper> logger)
    {
        _dbContext = dbContext;
        _dataService = dataService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ScrapeApiAsync()
    {
        var crates = await _dataService.GetCratesAsync();
        var prices = await _dataService.GetPricesAsync();

        await ProcessRaritiesAsync(crates);
        await ProcessCratesAndSkinsAsync(crates);
        await ProcessPricesAsync(prices);
    }

    private async Task ProcessRaritiesAsync(List<CrateDto> crates)
    {
        _dbContext.ChangeTracker.Clear();
        var existingRarities = await _dbContext.Rarities.ToDictionaryAsync(r => r.Id);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var uniqueRarities = ExtractUniqueRarities(crates);
            await SaveNewRaritiesAsync(uniqueRarities, existingRarities);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception($"Failed to process rarities: {ex.Message}", ex);
        }
    }

    private List<RarityDto> ExtractUniqueRarities(List<CrateDto> crates)
    {
        return [.. crates
            .SelectMany(c => c.contains?.Select(s => s.rarity) ?? [])
            .Concat(crates.SelectMany(c => c.contains_rare?.Select(s => s.rarity) ?? []))
            .Where(r => r != null)
            .DistinctBy(r => r?.id)
            .Cast<RarityDto>()];
    }

    private async Task SaveNewRaritiesAsync(List<RarityDto> rarities, Dictionary<string, Rarity> existingRarities)
    {
        foreach (var rarityDto in rarities)
        {
            if (rarityDto == null || existingRarities.ContainsKey(rarityDto.id))
                continue;

            var rarity = new Rarity
            {
                Id = rarityDto.id,
                Name = rarityDto.name,
                Color = rarityDto.color
            };
            _dbContext.Rarities.Add(rarity);
            existingRarities[rarity.Id] = rarity;
        }
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ProcessCratesAndSkinsAsync(List<CrateDto> crates)
    {
        _dbContext.ChangeTracker.Clear();
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var existingRarities = await _dbContext.Rarities.ToDictionaryAsync(r => r.Id);
            
            foreach (var crateDto in crates)
            {
                if (!AllowedTypes.Contains(crateDto.type ?? string.Empty))
                    continue;
                
                var crate = await GetOrCreateCrateAsync(crateDto);
                await ProcessCrateItemsAsync(crateDto, crate, existingRarities);
            }
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception($"Failed to process crates and skins: {ex.Message}", ex);
        }
    }

    private async Task<Crate> GetOrCreateCrateAsync(CrateDto crateDto)
    {
        var crate = await _dbContext.Crates.Include(c => c.Skins)
            .FirstOrDefaultAsync(c => c.Id == crateDto.id);
            
        if (crate != null)
            return crate;
            
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
        
        return crate;
    }

    private async Task ProcessCrateItemsAsync(CrateDto crateDto, Crate crate, Dictionary<string, Rarity> existingRarities)
    {
        var items = new List<ItemDto>();
        if (crateDto.contains != null) items.AddRange(crateDto.contains);
        if (crateDto.contains_rare != null) items.AddRange(crateDto.contains_rare);

        foreach (var itemDto in items.Where(s => s != null))
        {
            var skin = await GetOrCreateSkinAsync(itemDto, existingRarities);
            if (!crate.Skins.Any(s => s.Id == skin.Id))
                crate.Skins.Add(skin);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Skin> GetOrCreateSkinAsync(ItemDto itemDto, Dictionary<string, Rarity> existingRarities)
    {
        var skin = await _dbContext.Skins.FirstOrDefaultAsync(s => s.Id == itemDto.id);
        if (skin != null)
            return skin;
        
        skin = new Skin
        {
            Id = itemDto.id,
            Name = itemDto.name,
            RarityId = itemDto.rarity?.id,
            PaintIndex = itemDto.paint_index,
            Image = itemDto.image,
            MinFloat = double.TryParse(itemDto.min_float, out var minFloat) ? minFloat : (double?)null,
            MaxFloat = double.TryParse(itemDto.max_float, out var maxFloat) ? maxFloat : (double?)null,
            Souvenir = itemDto.souvenir,
            StatTrak = itemDto.stattrak,
            Category = itemDto.category?.id,
            Pattern = itemDto.pattern?.id
        };

        if (itemDto.rarity != null && !existingRarities.ContainsKey(itemDto.rarity.id))
        {
            var rarity = new Rarity
            {
                Id = itemDto.rarity.id,
                Name = itemDto.rarity.name,
                Color = itemDto.rarity.color
            };
            existingRarities[rarity.Id] = rarity;
            _dbContext.Rarities.Add(rarity);
        }

        _dbContext.Skins.Add(skin);
        return skin;
    }

    private async Task ProcessPricesAsync(Dictionary<string, PriceDetailDto> prices)
    {
        _dbContext.ChangeTracker.Clear();
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            await ProcessSkinPricesAsync(prices);
            await ProcessCratePricesAsync(prices);
            
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception($"Failed to process prices: {ex.Message}", ex);
        }
    }

    private async Task ProcessSkinPricesAsync(Dictionary<string, PriceDetailDto> prices)
    {
        var currentBatch = new List<Price>();
        var allSkins = await _dbContext.Skins.ToListAsync();
        
        var existingCombinations = await GetProcessedPriceCombinationsAsync();
        
        int exactMatches = 0;
        int substringMatches = 0;
        int noMatches = 0;
        int updatedPrices = 0;
        int newPrices = 0;
        
        var processedInThisBatch = new HashSet<(string SkinId, string Wear)>();
        
        foreach (var (originalKey, priceDetail) in prices)
        {
            var (skinName, wear, key) = ExtractSkinNameAndWear(originalKey);
            var skin = FindMatchingSkin(skinName, allSkins);
            
            if (skin == null)
            {
                noMatches++;
                continue;
            }
            
            if (string.Equals(skin.Name, skinName, StringComparison.OrdinalIgnoreCase))
                exactMatches++;
            else
                substringMatches++;
            
            if (existingCombinations.Contains((skin.Id, wear)) || processedInThisBatch.Contains((skin.Id, wear)))
            {
                if (existingCombinations.Contains((skin.Id, wear)))
                {
                    var existingPrice = await _dbContext.Prices.FirstOrDefaultAsync(p => 
                        p.SkinId == skin.Id && p.Wear_Category == wear);
                        
                    if (existingPrice != null)
                    {
                        UpdatePriceFromDetail(existingPrice, priceDetail);
                        updatedPrices++;
                    }
                }
                continue;
            }
            
            processedInThisBatch.Add((skin.Id, wear));
            
            var price = new Price
            {
                SkinId = skin.Id,
                Wear_Category = wear,
                Name = key
            };
            
            UpdatePriceFromDetail(price, priceDetail);
            _dbContext.Prices.Add(price);
            newPrices++;
                    
            currentBatch.Add(price);
            
            if (currentBatch.Count >= BatchSize)
            {
                await SavePriceBatchAsync(currentBatch);
                foreach (var combination in processedInThisBatch)
                    existingCombinations.Add(combination);
                processedInThisBatch.Clear();
            }
        }
        
        if (currentBatch.Count > 0)
        {
            await SavePriceBatchAsync(currentBatch);
        }
                
        _logger.LogInformation("Price processing summary - Exact matches: {ExactMatches}, Substring: {SubstringMatches}, Not Found: {NoMatches}, Updated: {UpdatedPrices}, New: {NewPrices}", 
            exactMatches, substringMatches, noMatches, updatedPrices, newPrices);
    }

    private async Task<(Price price, bool isNew)> GetOrCreateSkinPriceAsync(string skinId, string wear, string name, PriceDetailDto priceDetail)
    {
        var existingPrice = await _dbContext.Prices.FirstOrDefaultAsync(p => 
            p.SkinId == skinId && p.Wear_Category == wear);
            
        if (existingPrice != null)
        {
            UpdatePriceFromDetail(existingPrice, priceDetail);
            return (existingPrice, false);
        }
        
        var price = new Price
        {
            SkinId = skinId,
            Wear_Category = wear,
            Name = name
        };
        
        UpdatePriceFromDetail(price, priceDetail);
        _dbContext.Prices.Add(price);
        
        return (price, true);
    }

    private async Task ProcessCratePricesAsync(Dictionary<string, PriceDetailDto> prices)
    {
        int updatedCrates = 0;
        int newCratePrices = 0;
        
        foreach (var (key, priceDetail) in prices)
        {
            var crate = await _dbContext.Crates.FirstOrDefaultAsync(c => c.Name == key);
            if (crate == null)
                continue;
                
            var isNew = await UpdateCratePriceAsync(crate.Id, key, priceDetail);
            if (isNew)
                newCratePrices++;
            else
                updatedCrates++;
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Crate prices processed - Updated: {UpdatedCrates}, New: {NewCratePrices}", 
            updatedCrates, newCratePrices);
    }

    private async Task<bool> UpdateCratePriceAsync(string crateId, string name, PriceDetailDto priceDetail)
    {
        var cratePrice = await _dbContext.Prices.FirstOrDefaultAsync(p => p.CrateId == crateId);
        
        if (cratePrice != null)
        {
            UpdatePriceFromDetail(cratePrice, priceDetail);
            return false;
        }
        else
        {
            var newPrice = new Price
            {
                CrateId = crateId,
                Name = name
            };
            
            UpdatePriceFromDetail(newPrice, priceDetail);
            _dbContext.Prices.Add(newPrice);
            return true;
        }
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
    
    private (string SkinName, string Wear, string OriginalKey) ExtractSkinNameAndWear(string key)
    {
        var processedKey = key;
        
        if (key.Contains("Sticker", StringComparison.OrdinalIgnoreCase) ||
            key.Contains("Autograph", StringComparison.OrdinalIgnoreCase))
        {
            processedKey = NormalizePriceName(key);
        }

        string wear = ValidWears.FirstOrDefault(x => processedKey.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0) ?? "Default";
        
        string skinName = (wear != "Default")
            ? (processedKey.Contains('(')
               ? processedKey.Substring(0, processedKey.IndexOf('(')).Trim() 
               : processedKey.Replace(wear, "", StringComparison.OrdinalIgnoreCase).Trim())
            : processedKey.Trim();
            
        return (skinName, wear, key);
    }
    
    private static Skin? FindMatchingSkin(string skinName, List<Skin> allSkins)
    {
        var skin = allSkins.FirstOrDefault(s => 
            string.Equals(s.Name, skinName, StringComparison.OrdinalIgnoreCase));
            
        skin ??= allSkins.FirstOrDefault(s => s.Name != null && 
                (s.Name.Contains(skinName, StringComparison.OrdinalIgnoreCase) || 
                skinName.Contains(s.Name, StringComparison.OrdinalIgnoreCase)));
        
        if (skin?.RarityId?.Equals("rarity_ancient", StringComparison.OrdinalIgnoreCase) == true)
        {
            if (!string.IsNullOrEmpty(skin.Name) && !string.Equals(NormalizeAncientName(skin.Name), NormalizeAncientName(skinName), StringComparison.OrdinalIgnoreCase))
            {
                var altSkin = allSkins.FirstOrDefault(s => s.Name != null && 
                    string.Equals(NormalizeAncientName(s.Name), NormalizeAncientName(skinName), StringComparison.OrdinalIgnoreCase));
                    
                if (altSkin != null)
                {
                    return altSkin;
                }
            }
        }
        
        return skin;
    }
    
    private async Task<HashSet<(string SkinId, string Wear)>> GetProcessedPriceCombinationsAsync()
    {
        var processedCombinations = new HashSet<(string SkinId, string Wear)>();
        
        var existingPrices = await _dbContext.Prices
            .Where(p => p.SkinId != null)
            .Select(p => new { p.SkinId, p.Wear_Category })
            .ToListAsync();
            
        foreach (var existing in existingPrices)
        {
            if (existing.SkinId != null && !string.IsNullOrEmpty(existing.Wear_Category))
            {
                processedCombinations.Add((existing.SkinId, existing.Wear_Category));
            }
        }
        
        return processedCombinations;
    }
    
    private static void UpdatePriceFromDetail(Price price, PriceDetailDto priceDetail)
    {
        price.Steam_Last_24h = priceDetail.steam.last_24h;
        price.Steam_Last_7d = priceDetail.steam.last_7d;
        price.Steam_Last_30d = priceDetail.steam.last_30d;
        price.Steam_Last_90d = priceDetail.steam.last_90d;
        price.Steam_Last_Ever = priceDetail.steam.last_ever;
    }
    
    private async Task SavePriceBatchAsync(List<Price> batch)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();
            batch.Clear();
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
        {
            _logger.LogWarning("Duplicate key detected during price batch save: {Message}", ex.InnerException.Message);
            
            _dbContext.ChangeTracker.Clear();
            batch.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving price batch: {Message}", ex.Message);
            _dbContext.ChangeTracker.Clear();
            batch.Clear();
            throw;
        }
    }

    public async Task ScrapeApiWithMonitoringAsync()
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting API scrape at {Time}", startTime);
        
        try
        {
            await ScrapeApiAsync();
            
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("API scrape completed successfully in {Duration}", duration);
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "API scrape failed after {Duration}: {Message}", duration, ex.Message);
            throw;
        }
    }
}