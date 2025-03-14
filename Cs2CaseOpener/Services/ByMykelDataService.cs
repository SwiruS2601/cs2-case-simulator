using Cs2CaseOpener.Contracts;
using Cs2CaseOpener.Interfaces;
using System.Text.Json;

namespace Cs2CaseOpener.Services;

public class ByMykelDataService : IByMykelDataService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _cratesOptions;
    private readonly JsonSerializerOptions _pricesOptions;

    public ByMykelDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _cratesOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _cratesOptions.Converters.Add(new CustomDateTimeConverter());
        _pricesOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
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

        var prices = JsonSerializer.Deserialize<Dictionary<string, PriceDetailDto>>(content, _pricesOptions);
        
        return prices ?? throw new Exception("Failed to parse prices");
    }
}
