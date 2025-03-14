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
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public CrateController(CrateService caseService, IMemoryCache memoryCache)
    {
        _caseService = caseService;
        _cache = memoryCache;
        _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cached = _cache.Get("Crates");
        if (cached is not null)
            return Ok(cached);
        
        var crates = await _caseService.GetAllRelevantCrates();
        if (crates is null || crates.Count == 0)
            return NotFound("No crates found");

        _cache.Set("Crates", crates, _cacheOptions);
        return Ok(crates);
    }

    [HttpGet("cases")]
    public async Task<IActionResult> GetCases()
    {
        var cached = _cache.Get("Cases");
        if (cached is not null)
            return Ok(cached);

        var crates = await _caseService.GetCases();
        if (crates is null || crates.Count == 0)
            return NotFound("No cases found");

        _cache.Set("Cases", crates, _cacheOptions);
        return Ok(crates);
    }

    [HttpGet("souvenirs")]
    public async Task<IActionResult> GetSouvenirs()
    {
        var cached = _cache.Get("Souvenirs");
        if (cached is not null)
            return Ok(cached);

        var crates = await _caseService.GetSouvenirs();
        if (crates is null || crates.Count == 0)
            return NotFound("No souvenirs found");
        
        _cache.Set("Souvenirs", crates, _cacheOptions);
        return Ok(crates);
    }


    [HttpGet("stickers")]
    public async Task<IActionResult> GetStickers()
    {
        var cached = _cache.Get("Stickers");
        if (cached is not null)
            return Ok(cached);

        var crates = await _caseService.GetStickerCapsules();
        if (crates is null || crates.Count == 0)
            return NotFound("No sticker capsules found");

        _cache.Set("Stickers", crates, _cacheOptions);
        return Ok(crates);
    }

    [HttpGet("autographs")]
    public async Task<IActionResult> GetAutographs()
    {
        var cached = _cache.Get("Autographs");
        if (cached is not null)
            return Ok(cached);

        var crates = await _caseService.GetAutographCapsules();
        if (crates is null || crates.Count == 0)
            return NotFound("No autograph capsules found");

        _cache.Set("Autographs", crates, _cacheOptions);
        return Ok(crates);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var crate = await _caseService.GetById(id);
        return Ok(crate);
    }
        
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var decoded = Uri.UnescapeDataString(name);
        var  crate = await _caseService.GetByName(decoded);
        return Ok(crate);
    }

    [HttpGet("search/{searchKey}")]
    public async Task<IActionResult> Search(string searchKey)
    {
        var decoded = Uri.UnescapeDataString(searchKey);
        var crates = await _caseService.Search(decoded);
        return Ok(crates);
    }
}
