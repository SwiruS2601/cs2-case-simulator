using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/case")]
public class CaseController : ControllerBase
{
    private readonly CaseService _caseService;

    public CaseController(CaseService caseService)
    {
        _caseService = caseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCases()
    {
        var cases = await _caseService.GetCases();
        return Ok(cases);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCaseById(string id)
    {
        var case_ = await _caseService.GetCaseByIdAsync(id);
        return Ok(case_);
    }
}
