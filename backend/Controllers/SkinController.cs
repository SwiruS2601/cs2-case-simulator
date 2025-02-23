using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/skin")]
public class SkinController : ControllerBase
{

    private readonly SkinService _skinService;

    public SkinController(SkinService skinService)
    {
        _skinService = skinService;
    }

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
