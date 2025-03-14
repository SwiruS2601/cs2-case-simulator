using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Models;
public class Rarity
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [Column("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [Column("color")]
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    [JsonIgnore]
    public ICollection<Skin>? Skins { get; set; }

    // [JsonIgnore]
    // public ICollection<Sticker>? Stickers { get; set; }

    // [JsonIgnore]
    // public ICollection<Charm>? Charms { get; set; }
}
