using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/crate")]
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
        var cases = await _caseService.GetAll();
        return Ok(cases);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var crate = await _caseService.GetById(id);
        return Ok(crate);
    }

}
