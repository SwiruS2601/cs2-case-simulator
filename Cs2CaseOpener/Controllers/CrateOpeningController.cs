using Cs2CaseOpener.Models;
using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/crate-opening")]
public class CrateOpeningController : ControllerBase
{
    private readonly CrateOpeningService _crateOpeningService;
    private readonly IConfiguration _configuration;

    public CrateOpeningController(CrateOpeningService crateOpeningService, IConfiguration configuration)
    {
        _crateOpeningService = crateOpeningService;
        _configuration = configuration;
    }

    private bool IsAuthorized(string authHeader)
    {
         if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return false;
        }

        var token = authHeader["Bearer ".Length..].Trim();
        var expectedToken = _configuration["ApiSecrets:AuthToken"];
        
        if (token != expectedToken)
        {
            return false;
        }

        return true;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalOpeningCount()
    {
        try
        {
            int totalCount = await _crateOpeningService.GetTotalOpeningCountAsync();
            return Ok(new { totalCount });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Failed to retrieve opening count: " + ex.Message);
        }
    }

    [HttpPost]
    public IActionResult TrackOpening([FromHeader(Name = "Authorization")] string authHeader, CrateOpeningRequest request)
    {
        if (!IsAuthorized(authHeader))
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(request.CrateId) || string.IsNullOrEmpty(request.SkinId) || string.IsNullOrEmpty(request.ClientId))
            return BadRequest("CrateId, SkinId and ClientId are required");

        var clientIp = HttpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ??
            HttpContext.Connection.RemoteIpAddress?.ToString();
            
        var opening = new CrateOpening
        {
            CrateId = request.CrateId,
            SkinId = request.SkinId,
            ClientId = request.ClientId,
            ClientIp = clientIp,
            Rarity = request.Rarity,
            WearCategory = request.WearCategory,
            CrateName = request.CrateName,
            SkinName = request.SkinName,
            OpenedAt = DateTimeOffset.FromUnixTimeMilliseconds(request.Timestamp).UtcDateTime

        };

        _crateOpeningService.TrackOpening(opening);
        return Ok();
    }

    [HttpPost("batch")]
    public IActionResult TrackOpeningBatch([FromHeader(Name = "Authorization")] string authHeader, CrateOpeningBatchRequest request)
    {
        if (!IsAuthorized(authHeader))
        {
            return Unauthorized();
        }

        if (request.Openings == null || request.Openings.Count == 0)
            return BadRequest("No openings provided");

        if(string.IsNullOrEmpty(request.ClientId))
            return BadRequest("ClientId is required");

        var clientIp = HttpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ??
            HttpContext.Connection.RemoteIpAddress?.ToString();

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

            _crateOpeningService.TrackOpening(opening);
        }
        
        return Ok(new { ProcessedCount = request.Openings.Count });
    }
}

public record CrateOpeningRequest
{
    public required string CrateId { get; set; }
    public required string SkinId { get; set; }
    public string? ClientId { get; set; }
    public string? Rarity { get; set; }
    public string? WearCategory { get; set; }
    public string? CrateName { get; set; }
    public string? SkinName { get; set; }
    public long Timestamp { get; set; }
    
}

public record CrateOpeningBatchRequest
{
    public required List<CrateOpeningRequest> Openings { get; set; }
    public string? ClientId { get; set; }
}

