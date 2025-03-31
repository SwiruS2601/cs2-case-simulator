using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Cs2CaseOpener.Services
{
    public interface IIPGeolocationService
    {
        string GetCountryFromIP(string ipAddress);
    }

    public class MaxMindIPGeolocationService : IIPGeolocationService
    {
        private readonly DatabaseReader _reader;
        private readonly ILogger<MaxMindIPGeolocationService> _logger;

        public MaxMindIPGeolocationService(string dbPath, ILogger<MaxMindIPGeolocationService> logger)
        {
            _reader = new DatabaseReader(dbPath);
            _logger = logger;
        }

        public string GetCountryFromIP(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "localhost" || ipAddress == "127.0.0.1")
                return "Local";

            try
            {
                if (!IPAddress.TryParse(ipAddress, out var parsedIP))
                    return "Unknown";

                var response = _reader.Country(parsedIP);
                return response.Country.Name ?? "Unknown";
            }
            catch (AddressNotFoundException)
            {
                return "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up country for IP {IP}", ipAddress);
                return "Error";
            }
        }
    }
}