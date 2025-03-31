using Cs2CaseOpener.Data;
using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IIPGeolocationService _ipGeolocationService;

    private record CrateOpeningStatistic(string ClientIp, int RowCount, string Country);
    private record CrateOpeningStatisticDb(string? ClientIp, int RowCount);
    private record ClientStatistic(string ClientIp, int RowCount);
    private record CountryStatistic(string Country, int Count, IEnumerable<ClientStatistic> Entries);

    public StatisticsController(ApplicationDbContext dbContext, IIPGeolocationService ipGeolocationService)
    {
        _dbContext = dbContext;
        _ipGeolocationService = ipGeolocationService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dbResults = await _dbContext.Database
            .SqlQuery<CrateOpeningStatisticDb>($@"
                SELECT client_ip AS ClientIp, COUNT(*) AS RowCount
                FROM ""CrateOpenings""
                GROUP BY client_ip
                ORDER BY RowCount DESC")
            .ToListAsync();
                        
        var ipToCountryMap = new Dictionary<string, string>();
        foreach (var result in dbResults)
        {
            if (result.ClientIp != null && !ipToCountryMap.ContainsKey(result.ClientIp))
            {
                ipToCountryMap[result.ClientIp] = _ipGeolocationService.GetCountryFromIP(result.ClientIp);
            }
        }
        
        var enhancedResults = dbResults.Select(r => new CrateOpeningStatistic(
            r.ClientIp ?? "unknown", 
            r.RowCount, 
            ipToCountryMap[r.ClientIp ?? "unknown"]
        )).ToList();

        var countryStats = enhancedResults
            .GroupBy(r => r.Country)
            .Select(g => new CountryStatistic(
                Country: g.Key,
                Count: g.Sum(e => e.RowCount),
                Entries: g.Select(e => new ClientStatistic(e.ClientIp, e.RowCount))
            ))
            .OrderByDescending(c => c.Count)
            .ToList();

        return Ok(countryStats);
    }
}
