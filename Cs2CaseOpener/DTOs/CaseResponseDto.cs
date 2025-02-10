namespace Cs2CaseOpener.DTOs;

public class CaseResponseDto
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? FirstSaleDate { get; set; }
    public string? MarketHashName { get; set; }
    public bool? Rental { get; set; }
    public string? Image { get; set; }
    public string? ModelPlayer { get; set; }
    public List<CaseSkinDto>? Skins { get; set; }
}

public class CaseSkinDto
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? WeaponType { get; set; }
    public string? GunType { get; set; }
    public string? Rarity { get; set; }
    public string? RarityColor { get; set; }
    public object? Prices { get; set; }
    public string? FirstSaleDate { get; set; }
    public string? KnifeType { get; set; }
    public string? Image { get; set; }
    public double? MinFloat { get; set; }
    public double? MaxFloat { get; set; }
    public bool? Stattrak { get; set; }
}
