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
    public string? WeaponType { get; set; }
    public string? GunType { get; set; }
    public string? RarityColor { get; set; }
    public string? FirstSaleDate { get; set; }  // Changed from DateTime? to string
    public string? KnifeType { get; set; }
    public float? MinFloat { get; set; }
    public float? MaxFloat { get; set; }
    public string? CaseId { get; set; }  // Changed from int? to string
    public string? CaseName { get; set; }
}
