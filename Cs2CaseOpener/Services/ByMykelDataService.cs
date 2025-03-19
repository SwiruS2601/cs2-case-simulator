using Cs2CaseOpener.Contracts;
using System.Text.Json;

namespace Cs2CaseOpener.Services;

public class ByMykelDataService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _cratesOptions;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<ByMykelDataService> _logger;

    public ByMykelDataService(HttpClient httpClient, ILogger<ByMykelDataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cratesOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _cratesOptions.Converters.Add(new CustomDateTimeConverter());
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<List<CrateDto>> GetCratesAsync()
    {
        var response = await _httpClient.GetAsync("https://bymykel.github.io/CSGO-API/api/en/crates.json");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new Exception("Failed to fetch crates");
        }

        var crates = JsonSerializer.Deserialize<List<CrateDto>>(content, _cratesOptions);
        
        return crates ?? throw new Exception("Failed to parse crates");
    }

    public async Task<Dictionary<string, PriceDetailDto>> GetPricesAsync()
    {
        var response = await _httpClient.GetAsync("https://raw.githubusercontent.com/ByMykel/counter-strike-price-tracker/main/static/prices/latest.json");
        var content = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new Exception("Failed to fetch prices");
        }

        var prices = JsonSerializer.Deserialize<Dictionary<string, PriceDetailDto>>(content, _jsonOptions);
        
        return prices ?? throw new Exception("Failed to parse prices");
    }

    public async Task<List<SkinDetailDto>> GetAllSkinsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://bymykel.github.io/CSGO-API/api/en/skins.json");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var skinData = JsonSerializer.Deserialize<List<SkinDetailDto>>(content, _jsonOptions);
            
            return skinData ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch skin data from API");
            throw;
        }
    }
}
