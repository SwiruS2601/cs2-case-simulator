using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record CrateDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string? name,
    [property: JsonPropertyName("description")] string? description,
    [property: JsonPropertyName("type")] string? type,
    [property: JsonPropertyName("first_sale_date")] DateTime? first_sale_date,
    [property: JsonPropertyName("market_hash_name")] string? market_hash_name,
    [property: JsonPropertyName("rental")] bool? rental,
    [property: JsonPropertyName("image")] string? image,
    [property: JsonPropertyName("model_player")] string? model_player,
    [property: JsonPropertyName("contains")] List<ItemDto>? contains,
    [property: JsonPropertyName("contains_rare")] List<ItemDto>? contains_rare
);