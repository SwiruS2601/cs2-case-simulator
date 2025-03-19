using Cs2CaseOpener.Models;
using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;
using Cs2CaseOpener.Contracts;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/crate-opening")]
public class CrateOpeningController : ControllerBase
{
    private readonly CrateOpeningService _crateOpeningService;
    private readonly AuthorizationService _authService;

    public CrateOpeningController(
        CrateOpeningService crateOpeningService, 
        AuthorizationService authService)
    {
        _crateOpeningService = crateOpeningService;
        _authService = authService;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalOpeningCount()
    {
        try
        {
            long totalCount = await _crateOpeningService.GetTotalOpeningCountAsync();
            return Ok(new { totalCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Failed to retrieve opening count: " + ex.Message);
        }
    }

    [HttpPost("batch")]
    public IActionResult TrackOpeningBatch([FromHeader(Name = "Authorization")] string authHeader, CrateOpeningBatchRequest request)
    {
        if (!_authService.IsAuthorized(authHeader))
        {
            return Unauthorized();
        }

        if (request.Openings == null || request.Openings.Count == 0)
            return BadRequest("No openings provided");

        if(string.IsNullOrEmpty(request.ClientId))
            return BadRequest("ClientId is required");

        var clientIp = HttpContext.Request.Headers["X-Client-IP"].FirstOrDefault();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ??
                HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        var openings = new List<CrateOpening>(request.Openings.Count);
        foreach (var item in request.Openings)
        {
            var opening = new CrateOpening
            {
                CrateId = item.CrateId,
                SkinId = item.SkinId,
                ClientId = request.ClientId,
                ClientIp = clientIp,
                Rarity = item.Rarity,
                WearCategory = item.WearCategory,
                CrateName = item.CrateName,
                SkinName = item.SkinName,
                OpenedAt = DateTimeOffset.FromUnixTimeMilliseconds(item.Timestamp).UtcDateTime
            };
            
            openings.Add(opening);
        }
        
        _crateOpeningService.TrackOpeningBatch(openings);
        
        return Ok(new { ProcessedCount = request.Openings.Count });
    }
}


