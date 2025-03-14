using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record PriceDetailDto(
    [property: JsonPropertyName("steam")] SteamPriceDto steam
);