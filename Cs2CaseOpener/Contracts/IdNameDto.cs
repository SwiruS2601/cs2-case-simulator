using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Contracts;

public record IdNameDto(
    [property: JsonPropertyName("id")] string id,
    [property: JsonPropertyName("name")] string name
);
