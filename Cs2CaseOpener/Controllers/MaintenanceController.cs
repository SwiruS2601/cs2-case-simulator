using Cs2CaseOpener.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cs2CaseOpener.Controllers;

[ApiController]
[Route("api/maintenance")]
public class MaintenanceController : ControllerBase
{
    private readonly DataRetentionService _dataRetentionService;
    private readonly AuthorizationService _authService;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(
        DataRetentionService dataRetentionService,
        AuthorizationService authService,
        ILogger<MaintenanceController> logger)
    {
        _dataRetentionService = dataRetentionService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("cleanup")]
    public async Task<IActionResult> CleanupDatabase([FromHeader(Name = "Authorization")] string authHeader)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        if (!IsLocalNetwork(clientIp))
        {
            _logger.LogWarning("Unauthorized maintenance attempt from non-local IP: {IP}", clientIp);
            return Unauthorized("This operation is only allowed from the local network");
        }

        if (!_authService.IsAuthorized(authHeader))
        {
            _logger.LogWarning("Unauthorized maintenance attempt: invalid token");
            return Unauthorized("Invalid authorization token");
        }

        try
        {
            _logger.LogInformation("Manual database cleanup triggered");
            
            await _dataRetentionService.PurgeOldData(DateTime.UtcNow);
            
            return Ok(new { 
                Status = "Success", 
                Message = "Database cleanup completed successfully", 
                Timestamp = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual database maintenance");
            return StatusCode(500, $"Maintenance operation failed: {ex.Message}");
        }
    }
    
    private bool IsLocalNetwork(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            return false;
            
        if (ipAddress == "127.0.0.1" || ipAddress == "::1")
            return true;
            
        if (ipAddress.StartsWith("192.168.") || ipAddress.StartsWith("10.") || ipAddress.StartsWith("::ffff:10."))
            return true;
            
        return false;
    }
}