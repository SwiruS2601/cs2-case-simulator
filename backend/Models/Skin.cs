using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Models;
public class Skin
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [Column("rarity_id")]
    [JsonPropertyName("rarity_id")]
    public string? RarityId { get; set; }

    [Column("paint_index")]
    [JsonPropertyName("paint_index")]
    public string? PaintIndex { get; set; }

    [Column("image")]
    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [Column("min_float")]
    [JsonPropertyName("min_float")]
    public double? MinFloat { get; set; }

    [Column("max_float")]
    [JsonPropertyName("max_float")]
    public double? MaxFloat { get; set; }

    [Column("stattrak")]
    [JsonPropertyName("stattrak")]
    public bool? StatTrak { get; set; }

    [Column("souvenir")]
    [JsonPropertyName("souvenir")]
    public bool? Souvenir { get; set; }

    [Column("category")]
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [Column("pattern")]
    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }
    
    public ICollection<Price>? Prices { get; set; }

    [JsonIgnore]
    public ICollection<Crate>? Crates { get; set; }
    public Rarity? Rarity { get; set; }
}