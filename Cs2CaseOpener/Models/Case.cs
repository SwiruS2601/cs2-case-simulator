using System;
using System.ComponentModel.DataAnnotations;

namespace Cs2CaseOpener.Models;

public class Case
{
    [Key]
    public required string Id { get; set; }
    
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? FirstSaleDate { get; set; }
    public string? MarketHashName { get; set; }
    public bool? Rental { get; set; }
    public string? Image { get; set; }
    public string? ModelPlayer { get; set; }
    
    // One-to-many relationship: a Case has many Skins.
    public ICollection<Skin> Skins { get; set; } = new List<Skin>();
}
