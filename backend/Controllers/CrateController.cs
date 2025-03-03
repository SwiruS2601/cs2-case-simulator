using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/crates")]
public class CrateController : ControllerBase
{
    private readonly CrateService _caseService;

    public CrateController(CrateService caseService)
    {
        _caseService = caseService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var crates = await _caseService.GetAllRelevantCrates();
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
