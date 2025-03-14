namespace Cs2CaseOpener.Interfaces;

public interface IApiScraper
{
    Task ScrapeApiAsync();
    Task ScrapeApiWithMonitoringAsync();
}
