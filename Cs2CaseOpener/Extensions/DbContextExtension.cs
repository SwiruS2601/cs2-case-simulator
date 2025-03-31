using Cs2CaseOpener.Data;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Extensions;

public static class DbContextExtension
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        return services;
    }
}