using Cs2CaseOpener.Contracts;

namespace Cs2CaseOpener.Interfaces;

public interface IByMykelDataService
{
    Task<List<CrateDto>> GetCratesAsync();
    Task<Dictionary<string, PriceDetailDto>> GetPricesAsync();
}
