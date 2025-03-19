using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/rarity")]
public class RarityController(RarityService rarityService) : ControllerBase
{
    private readonly RarityService _rarityService = rarityService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cases = await _rarityService.GetAll();
        return Ok(cases);
    }

}
