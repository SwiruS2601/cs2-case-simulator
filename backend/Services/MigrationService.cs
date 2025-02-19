using System.Text.Json;
using Cs2CaseOpener.DB;
using Cs2CaseOpener.DTOs;
using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class MigrationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public MigrationService(ApplicationDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task MigrateDataFromApiAsync()
    {
        try
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();

            var existingSkins = await _dbContext.Skins.ToDictionaryAsync(s => s.Id);
            var existingCases = new Dictionary<string, Case>();

            var relationsResponse = await _httpClient.GetAsync("http://localhost:3000/api/crate-skin-relations");
            relationsResponse.EnsureSuccessStatusCode();
            var relationsJson = await relationsResponse.Content.ReadAsStringAsync();
            var relations = JsonSerializer.Deserialize<List<CrateSkinRelationDTO>>(relationsJson, _jsonOptions);

            var crateListResponse = await _httpClient.GetAsync("http://localhost:3000/api/crates");
            crateListResponse.EnsureSuccessStatusCode();
            var crateListJson = await crateListResponse.Content.ReadAsStringAsync();

            var crateDtos = JsonSerializer.Deserialize<List<CaseDTO>>(crateListJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (crateDtos == null)
            {
                Console.WriteLine("No crates found.");
                return;
            }

            var batchSize = 10;
            for (var i = 0; i < crateDtos.Count; i += batchSize)
            {
                var batch = crateDtos.Skip(i).Take(batchSize);
                var tasks = batch.Select(async crateDto =>
                {
                    var detailResponse = await _httpClient.GetAsync($"http://localhost:3000/api/crates/{crateDto.Id}");
                    detailResponse.EnsureSuccessStatusCode();
                    var detailJson = await detailResponse.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CrateDetailDTO>(detailJson, _jsonOptions);
                });

                var crateDetails = (await Task.WhenAll(tasks)).Where(x => x != null);

                foreach (var crateDetail in crateDetails)
                {
                    var existingCase = await _dbContext.Cases
                        .Include(c => c.Skins)
                        .FirstOrDefaultAsync(c => c.Id == crateDetail.Id);

                    if (existingCase == null)
                    {
                        existingCase = new Case
                        {
                            Id = crateDetail.Id,
                            Skins = new List<Skin>()
                        };
                        _dbContext.Cases.Add(existingCase);
                    }

                    existingCases[existingCase.Id] = existingCase;

                    existingCase.Name = crateDetail.Name;
                    existingCase.Type = crateDetail.Type;
                    existingCase.FirstSaleDate = crateDetail.FirstSaleDate;
                    existingCase.MarketHashName = crateDetail.Market_Hash_Name;
                    existingCase.Rental = crateDetail.Rental;
                    existingCase.Image = crateDetail.Image;
                    existingCase.ModelPlayer = crateDetail.Model_Player;

                    if (crateDetail.Skins != null)
                    {
                        foreach (var skinDto in crateDetail.Skins)
                        {
                            Skin skin;
                            if (existingSkins.TryGetValue(skinDto.Id, out var existingSkin))
                            {
                                skin = existingSkin;
                            }
                            else
                            {
                                skin = new Skin { Id = skinDto.Id };
                                existingSkins.Add(skinDto.Id, skin);
                                _dbContext.Skins.Add(skin);
                            }

                            skin.Name = skinDto.Name;
                            skin.Classid = skinDto.Classid;
                            skin.Type = skinDto.Type;
                            skin.WeaponType = skinDto.Weapon_Type;
                            skin.GunType = skinDto.Gun_Type;
                            skin.Rarity = skinDto.Rarity;
                            skin.RarityColor = skinDto.Rarity_Color;
                            if (skinDto.Prices != null)
                            {
                                skin.Prices = JsonSerializer.Serialize(skinDto.Prices, _jsonOptions);
                            }
                            skin.FirstSaleDate = skinDto.First_Sale_Date;
                            skin.KnifeType = skinDto.Knife_Type;
                            skin.Image = skinDto.Image;
                            skin.MinFloat = skinDto.Min_Float;
                            skin.MaxFloat = skinDto.Max_Float;
                            skin.Stattrak = skinDto.Stattrak;

                            if (!existingCase.Skins.Contains(skin))
                            {
                                existingCase.Skins.Add(skin);
                            }
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            if (relations != null)
            {
                foreach (var relation in relations)
                {
                    if (existingCases.TryGetValue(relation.A, out var case_) &&
                        existingSkins.TryGetValue(relation.B, out var skin))
                    {
                        if (!case_.Skins.Contains(skin))
                        {
                            case_.Skins.Add(skin);
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            Console.WriteLine("Data migration completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during migration: {ex.Message}");
            throw;
        }
    }
}