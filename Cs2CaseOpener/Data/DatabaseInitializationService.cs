using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Data;

public class DatabaseInitializationService
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseInitializationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.MigrateAsync();
    }
}
