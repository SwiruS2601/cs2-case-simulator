using Cs2CaseOpener.Contracts;
using Cs2CaseOpener.Data;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class ApiScraper
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ByMykelDataService _dataService;
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<ApiScraper> _logger;
    private readonly string[] ValidWears = ["Minimal Wear", "Field-Tested", "Battle-Scarred", "Well-Worn", "Factory New"];
    private readonly string[] AllowedTypes = ["Case", "Souvenir", "Sticker Capsule", "Autograph Capsule"];
    private const int BatchSize = 100;

    public ApiScraper(ApplicationDbContext dbContext, ByMykelDataService dataService, UnitOfWork unitOfWork, ILogger<ApiScraper> logger)
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
        var skins = await _dataService.GetAllSkinsAsync();

        await ProcessRaritiesAsync(crates);
        await ProcessCratesAndSkinsAsync(crates, skins);
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

    private async Task ProcessCratesAndSkinsAsync(List<CrateDto> crates, List<SkinDetailDto> skinDetails)
    {
        _dbContext.ChangeTracker.Clear();
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var existingRarities = await _dbContext.Rarities.ToDictionaryAsync(r => r.Id);
            
            var skinDetailsDict = skinDetails
                .Where(s => s != null)
                .ToDictionary(s => s.id, s => s);
            
            foreach (var crateDto in crates)
            {
                if (!AllowedTypes.Contains(crateDto.type ?? string.Empty))
                    continue;
                
                var crate = await GetOrCreateCrateAsync(crateDto);
                await ProcessCrateItemsAsync(crateDto, crate, existingRarities, skinDetailsDict);
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

    private async Task ProcessCrateItemsAsync(CrateDto crateDto, Crate crate, 
        Dictionary<string, Rarity> existingRarities, 
        Dictionary<string, SkinDetailDto> skinDetailsDict)
    {
        var items = new List<ItemDto>();
        if (crateDto.contains != null) items.AddRange(crateDto.contains);
        if (crateDto.contains_rare != null) items.AddRange(crateDto.contains_rare);

        foreach (var itemDto in items.Where(s => s != null))
        {
            var skin = await GetOrCreateSkinAsync(itemDto, existingRarities, skinDetailsDict);
            if (!crate.Skins.Any(s => s.Id == skin.Id))
                crate.Skins.Add(skin);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Skin> GetOrCreateSkinAsync(ItemDto itemDto, 
        Dictionary<string, Rarity> existingRarities,
        Dictionary<string, SkinDetailDto> skinDetailsDict)
    {
        var skin = await _dbContext.Skins.FirstOrDefaultAsync(s => s.Id == itemDto.id);
        
        if (skin == null)
        {
            skin = await _dbContext.Skins.FirstOrDefaultAsync(s => 
                s.Name == itemDto.name && 
                s.PaintIndex == itemDto.paint_index);
        }
        
        if (skin != null)
            return skin;
        
        skinDetailsDict.TryGetValue(itemDto.id, out var skinDetails);
        
        skin = new Skin
        {
            Id = itemDto.id,
            Name = itemDto.name,
            RarityId = itemDto.rarity?.id,
            PaintIndex = itemDto.paint_index,
            Image = itemDto.image,
            MinFloat = skinDetails?.min_float ?? null,
            MaxFloat = skinDetails?.max_float ?? null,
            Souvenir = skinDetails?.souvenir ?? null,
            StatTrak = skinDetails?.stattrak ?? null,
            Category = skinDetails?.category?.name ?? null,
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

    public async Task UpdateNullSkinPropertiesAsync()
    {
        _dbContext.ChangeTracker.Clear();
        _logger.LogInformation("Starting update of skins with null properties");
        
        var crates = await _dataService.GetCratesAsync();
        var skinData = await _dataService.GetAllSkinsAsync();
        
        var allItems = new List<ItemDto>();
        foreach (var crateDto in crates)
        {
            if (crateDto.contains != null) allItems.AddRange(crateDto.contains);
            if (crateDto.contains_rare != null) allItems.AddRange(crateDto.contains_rare);
        }
        
        var itemsDict = allItems
            .Where(item => item != null)
            .DistinctBy(item => item.id)
            .ToDictionary(item => item.id, item => item);
        
        var skinDetailsDict = skinData
            .Where(s => s != null)
            .ToDictionary(s => s.id, s => s);
        
        var skinsToUpdate = await _dbContext.Skins
            .Where(s => 
                s.MinFloat == null || 
                s.MaxFloat == null || 
                s.Souvenir == null || 
                s.StatTrak == null || 
                s.Category == null)
            .ToListAsync();
        
        _logger.LogInformation("Found {Count} skins with null properties to update", skinsToUpdate.Count);
        
        int updatedCount = 0;
        int notFoundCount = 0;
        int categoryUpdates = 0;
        
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            foreach (var skin in skinsToUpdate)
            {
                bool updated = false;
                
                if (skinDetailsDict.TryGetValue(skin.Id, out var skinDetail))
                {
                    if (skinDetail.min_float.HasValue)
                        skin.MinFloat = skinDetail.min_float;
                        
                    if (skinDetail.max_float.HasValue)
                        skin.MaxFloat = skinDetail.max_float;
                        
                    if (skinDetail.souvenir.HasValue)
                        skin.Souvenir = skinDetail.souvenir;
                        
                    if (skinDetail.stattrak.HasValue)
                        skin.StatTrak = skinDetail.stattrak;
                        
                    if (skinDetail.category != null && !string.IsNullOrEmpty(skinDetail.category.name))
                    {
                        skin.Category = skinDetail.category.name;
                        categoryUpdates++;
                    }
                    
                    updated = true;
                }
                
                if (updated)
                {
                    updatedCount++;
                    
                    if (updatedCount % 100 == 0)
                    {
                        _logger.LogInformation("Updated {Count} skins so far", updatedCount);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                else
                {
                    notFoundCount++;
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation("Completed updating skins. Updated: {UpdatedCount}, Categories updated: {CategoryUpdates}, Not found in API data: {NotFoundCount}", 
                updatedCount, categoryUpdates, notFoundCount);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error updating skin properties: {Message}", ex.Message);
            throw;
        }
    }

    public async Task CleanupDuplicateSkinsAsync()
    {
        _dbContext.ChangeTracker.Clear();
        await _unitOfWork.BeginTransactionAsync();
        _logger.LogInformation("Starting duplicate skin cleanup process.");

        try
        {
            var duplicateGroups = await _dbContext.Skins
                .GroupBy(s => new { s.Name, s.PaintIndex })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToListAsync();

            _logger.LogInformation("Found {Count} groups of duplicate skins based on Name and PaintIndex.", duplicateGroups.Count);
            int totalDeleted = 0;

            foreach (var groupKey in duplicateGroups)
            {
                // Important: Load prices and crates for accurate comparison
                var duplicates = await _dbContext.Skins
                    .Include(s => s.Prices)
                    .Include(s => s.Crates) // Assuming a navigation property 'Crates' exists
                    .Where(s => s.Name == groupKey.Name && s.PaintIndex == groupKey.PaintIndex)
                    .OrderBy(s => s.Id) // Consistent ordering for tie-breaking
                    .ToListAsync();

                if (duplicates.Count <= 1) continue; // Should not happen based on query, but safe check

                // Determine the skin to keep
                Skin skinToKeep = duplicates
                    .OrderByDescending(s => s.Prices.Any()) // Prioritize skins with prices
                    .ThenByDescending(s => s.Crates.Count) // Then by number of crate associations
                    .First(); // If still tied, OrderBy(s => s.Id) picks the one with the 'lowest' ID
                
                _logger.LogDebug("Duplicate group: Name='{Name}', PaintIndex='{PaintIndex}'. Keeping SkinId='{SkinId}'. Deleting {DeleteCount} others.", 
                    groupKey.Name, groupKey.PaintIndex, skinToKeep.Id, duplicates.Count - 1);

                // Identify skins to delete
                var skinsToDelete = duplicates.Where(s => s.Id != skinToKeep.Id).ToList();

                if (skinsToDelete.Any())
                {
                    _dbContext.Skins.RemoveRange(skinsToDelete);
                    totalDeleted += skinsToDelete.Count;
                }
            }

            if (totalDeleted > 0)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted {Count} duplicate skin entries.", totalDeleted);
            }
            else
            {
                _logger.LogInformation("No duplicate skins needed deletion.");
            }
            
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {   
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error during duplicate skin cleanup: {Message}", ex.Message);
            throw;
        }
    }
}