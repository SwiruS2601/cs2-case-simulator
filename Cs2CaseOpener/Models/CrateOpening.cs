using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cs2CaseOpener.Models;

public class CrateOpening
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public required string CrateId { get; set; }
    
    [Required]
    public required string SkinId { get; set; }
    
    public required string ClientId { get; set; }
    
    public string? ClientIp { get; set; }
    public string? Rarity { get; set; }
    public string? WearCategory { get; set; }
    public string? CrateName { get; set; }
    public string? SkinName { get; set; } 
    
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Crate? Crate { get; set; }
    public virtual Skin? Skin { get; set; }
}