namespace Cs2CaseOpener.DTOs;

public class CrateDetailDto
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? FirstSaleDate { get; set; }
    public string? Market_Hash_Name { get; set; }
    public bool? Rental { get; set; }
    public string? Image { get; set; }
    public string? Model_Player { get; set; }
    public List<SkinDetailsDTO>? Skins { get; set; }
}

public class SkinDetailsDTO
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Classid { get; set; }
    public string? Type { get; set; }
    public string? Weapon_Type { get; set; }
    public string? Gun_Type { get; set; }
    public string? Rarity { get; set; }
    public string? Rarity_Color { get; set; }
    public object? Prices { get; set; }
    public string? First_Sale_Date { get; set; }
    public string? Knife_Type { get; set; }
    public string? Image { get; set; }
    public double? Min_Float { get; set; }
    public double? Max_Float { get; set; }
    public bool? Stattrak { get; set; }
}
