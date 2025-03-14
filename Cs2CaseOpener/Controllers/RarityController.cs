using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/rarity")]
public class RarityController : ControllerBase
{
    private readonly RarityService _rarityService;

    public RarityController(RarityService rarityService)
    {
        _rarityService = rarityService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cases = await _rarityService.GetAll();
        return Ok(cases);
    }

}
