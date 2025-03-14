using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record SteamPriceDto(
    [property: JsonPropertyName("last_24h")] double? last_24h,
    [property: JsonPropertyName("last_7d")] double? last_7d,
    [property: JsonPropertyName("last_30d")] double? last_30d,
    [property: JsonPropertyName("last_90d")] double? last_90d,
    [property: JsonPropertyName("last_ever")] double? last_ever
);