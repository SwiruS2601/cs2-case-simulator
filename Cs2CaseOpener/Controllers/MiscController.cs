using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/misc")]
public class MiscController : ControllerBase
{
    private readonly ApiScraper _apiScraper;
    private readonly AuthorizationService _authService;

    public MiscController(ApiScraper apiScraper, AuthorizationService authService)
    {
        _apiScraper = apiScraper;
        _authService = authService;
    }

    [HttpGet("update-skin-properties")]
    public async Task<IActionResult> UpdateSkinProperties([FromHeader(Name = "Authorization")] string authHeader)
    {
        if (!_authService.IsAuthorized(authHeader))
        {
            return Unauthorized();
        }

        try
        {
            await _apiScraper.UpdateNullSkinPropertiesAsync();
            return Ok("Skin properties updated successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating skin properties: {ex.Message}");
        }
    }
}