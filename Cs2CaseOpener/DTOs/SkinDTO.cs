using System.Text.Json.Nodes; // if you want to work with JSON nodes

namespace Cs2CaseOpener.DTOs;

public class SkinDTO
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Classid { get; set; }
    public string? Type { get; set; }
    public string? Weapon_Type { get; set; }
    public string? Gun_Type { get; set; }
    public string? Rarity { get; set; }
    public string? Rarity_Color { get; set; }
    public JsonObject? Prices { get; set; }
    public string? First_Sale_Date { get; set; }
    public string? Knife_Type { get; set; }
    public string? Image { get; set; }
    public double? Min_Float { get; set; }
    public double? Max_Float { get; set; }
    public bool? Stattrak { get; set; }
}
