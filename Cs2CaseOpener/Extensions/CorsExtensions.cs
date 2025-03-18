namespace Cs2CaseOpener.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
            {
                policy.WithOrigins(
                    "http://localhost", 
                    "http://localhost:3000", 
                    "http://localhost:5173", 
                    "http://10.10.10.46:5020", 
                    "https://case.oki.gg"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });
        
        return services;
    }
}