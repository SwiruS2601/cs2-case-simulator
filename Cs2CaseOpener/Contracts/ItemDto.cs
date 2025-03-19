using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record ItemDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string? name,
    [property: JsonPropertyName("rarity")] RarityDto? rarity,
    [property: JsonPropertyName("paint_index")] string? paint_index,
    [property: JsonPropertyName("image")] string? image
);
