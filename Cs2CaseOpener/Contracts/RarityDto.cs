using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;
   
public record RarityDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string name,
    [property: JsonPropertyName("color")] string color
);