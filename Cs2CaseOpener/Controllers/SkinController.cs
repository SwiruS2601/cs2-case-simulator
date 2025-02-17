using Cs2CaseOpener.DB;
using Cs2CaseOpener.DTOs;
using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/skin")]
public class SkinController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SkinService _skinService;

    public SkinController(ApplicationDbContext dbContext, SkinService skinService)
    {
        _dbContext = dbContext;
        _skinService = skinService;
    }

    [HttpGet]
    [ResponseCache(Duration = 300)] // 5 minutes cache
    public async Task<ActionResult<IEnumerable<SkinDTO>>> GetAll()
    {
        var skins = await _skinService.GetAllSkinsAsync();
        return Ok(skins);
    }

    [HttpGet("{id}")]
    public IActionResult GetSkinById(string id)
    {
        var skin = _skinService.GetSkinByIdAsync(id);
        return Ok(skin);
    }

}
