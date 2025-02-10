using Cs2CaseOpener.DB;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.Services;

public class DatabaseInitializationService
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseInitializationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync()
    {
        using var connection = (SqliteConnection)_dbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode=WAL;";
        await command.ExecuteNonQueryAsync();
    }
}
