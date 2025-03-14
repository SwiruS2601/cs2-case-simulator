using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record ItemDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string? name,
    [property: JsonPropertyName("rarity")] RarityDto? rarity,
    [property: JsonPropertyName("paint_index")] string? paint_index,
    [property: JsonPropertyName("image")] string? image,
    [property: JsonPropertyName("stattrak")] bool? stattrak,
    [property: JsonPropertyName("souvenir")] bool? souvenir,
    [property: JsonPropertyName("min_float")] string? min_float,
    [property: JsonPropertyName("max_float")] string? max_float,
    [property: JsonPropertyName("category")] IdNameDto? category,
    [property: JsonPropertyName("pattern")] IdNameDto? pattern
);
