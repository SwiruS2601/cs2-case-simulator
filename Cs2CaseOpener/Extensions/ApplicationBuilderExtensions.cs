using Cs2CaseOpener.Middleware;
using Cs2CaseOpener.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Cs2CaseOpener.Services;

namespace Cs2CaseOpener.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog(dispose: true);
        
        return builder;
    }

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseRequestLogging();
        
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseCors("AllowLocalhost");

        app.UseSwaggerWhenDevelopment();

        app.MapControllers();
        
        return app;
    }
    
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            var dbInitializer = services.GetRequiredService<DatabaseInitializationService>();
            await dbInitializer.InitializeAsync();
            
            // var apiScraper = services.GetRequiredService<IApiScraper>();
            // await apiScraper.ScrapeApiAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    public static async Task InitializeCountersAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
    
        try
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Initializing crate opening counter...");
        
            var crateOpeningService = services.GetRequiredService<CrateOpeningService>();
            await crateOpeningService.InitializeCounterAsync();
        
            logger.LogInformation("Crate opening counter initialized successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the crate opening counter");
        }
    }

    public static WebApplication UseSwaggerWhenDevelopment(this WebApplication app)
    {
   if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CS2 Case Opener API v1");
                c.RoutePrefix = "swagger";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.EnableDeepLinking();
                c.DisplayRequestDuration();
            });
        }
        
        return app;
    }
}