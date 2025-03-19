using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/skins")]
public class SkinController(SkinService skinService) : ControllerBase
{

    private readonly SkinService _skinService = skinService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var skins = await _skinService.GetAll();
        return Ok(skins);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var skin = await _skinService.GetById(id);
        return Ok(skin);
    }
}
