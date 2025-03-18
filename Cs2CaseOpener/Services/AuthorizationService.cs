
namespace Cs2CaseOpener.Services;

public class AuthorizationService
{
    private readonly IConfiguration _configuration;

    public AuthorizationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsAuthorized(string authHeader)
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
    
}