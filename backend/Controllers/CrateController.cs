using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/crates")]
public class CrateController : ControllerBase
{
    private readonly CrateService _caseService;
    private readonly IMemoryCache _cache;
    private const string CratesCacheKey = "CratesCacheKey";

    public CrateController(CrateService caseService, IMemoryCache cache)
    {
        _caseService = caseService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!_cache.TryGetValue(CratesCacheKey, out var crates))
        {
            crates = await _caseService.GetAll();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(60));
                
            _cache.Set(CratesCacheKey, crates, cacheOptions);
        }
        
        return Ok(crates);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        string cacheKey = $"Crate_{id}";

        if (!_cache.TryGetValue(cacheKey, out object? crate))
        {
            crate = await _caseService.GetById(id);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(60));

            _cache.Set(cacheKey, crate, cacheOptions);
        }

        return Ok(crate);
    }

}
