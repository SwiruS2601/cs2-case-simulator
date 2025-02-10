using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Cs2CaseOpener.Models;

public class Skin
{
    [Key]
    public required string Id { get; set; }
    
    public string? Name { get; set; }
    public string? Classid { get; set; }
    public string? Type { get; set; }
    public string? WeaponType { get; set; }
    public string? GunType { get; set; }
    public string? Rarity { get; set; }
    public string? RarityColor { get; set; }
    
    private string? _prices;
    public string? Prices
    {
        get => _prices;
        set
        {
            _prices = value;
            if (value != null)
            {
                try
                {
                    _parsedPrices = JsonSerializer.Deserialize<SkinPrice>(value);
                }
                catch
                {
                    _parsedPrices = null;
                }
            }
        }
    }

    private SkinPrice? _parsedPrices;
    public SkinPrice? ParsedPrices
    {
        get
        {
            if (_parsedPrices == null && Prices != null)
            {
                try
                {
                    _parsedPrices = JsonSerializer.Deserialize<SkinPrice>(Prices);
                }
                catch
                {
                    _parsedPrices = null;
                }
            }
            return _parsedPrices;
        }
    }

    public string? FirstSaleDate { get; set; }
    public string? KnifeType { get; set; }
    public string? Image { get; set; }
    public double? MinFloat { get; set; }
    public double? MaxFloat { get; set; }
    public bool? Stattrak { get; set; }
    
    // Foreign key linking back to the Case.
    public string? CaseId { get; set; }
    public Case Case { get; set; }
}