using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cs2CaseOpener.Models;
public class Price
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Column("skin_id")]
    [JsonPropertyName("skin_id")]
    public string? SkinId { get; set; }

    [Column("crate_id")]
    [JsonPropertyName("crate_id")]
    public string? CrateId { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore]
    public Crate? Crate { get; set; }

    [JsonIgnore]
    public Skin? Skin { get; set; }

    [Column("wear_category")]
    [JsonPropertyName("wear_category")]
    public string? Wear_Category { get; set; }

    [Column("steam_last_24h")]
    [JsonPropertyName("steam_last_24h")]
    public double? Steam_Last_24h { get; set; }

    [Column("steam_last_7d")]
    [JsonPropertyName("steam_last_7d")]
    public double? Steam_Last_7d { get; set; }

    [Column("steam_last_30d")]
    [JsonPropertyName("steam_last_30d")]
    public double? Steam_Last_30d { get; set; }

    [Column("steam_last_90d")]
    [JsonPropertyName("steam_last_90d")]
    public double? Steam_Last_90d { get; set; }

    [Column("steam_last_ever")]
    [JsonPropertyName("steam_last_ever")]
    public double? Steam_Last_Ever { get; set; }
}