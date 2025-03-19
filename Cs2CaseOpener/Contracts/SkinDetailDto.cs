using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record SkinDetailDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string name,
    [property: JsonPropertyName("description")] string description,
    [property: JsonPropertyName("weapon")] WeaponDetailDto? weapon,
    [property: JsonPropertyName("category")] CategoryDetailDto? category,
    [property: JsonPropertyName("min_float")] double? min_float,
    [property: JsonPropertyName("max_float")] double? max_float,
    [property: JsonPropertyName("stattrak")] bool? stattrak,
    [property: JsonPropertyName("souvenir")] bool? souvenir,
    [property: JsonPropertyName("paint_index")] string paint_index,
    [property: JsonPropertyName("wears")] List<WearDetailDto>? wears
);

public record WeaponDetailDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("weapon_id")] int weapon_id,
    [property: JsonPropertyName("name")] string name
);

public record CategoryDetailDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string name
);

public record PatternDetailDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string name
);

public record WearDetailDto(
    [property: JsonPropertyName("id")]  string id,
    [property: JsonPropertyName("name")]  string name
);