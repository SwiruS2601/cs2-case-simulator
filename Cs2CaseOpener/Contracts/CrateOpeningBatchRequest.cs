namespace Cs2CaseOpener.Contracts;

public record CrateOpeningRequest
{
    public required string CrateId { get; set; }
    public required string SkinId { get; set; }
    public string? ClientId { get; set; }
    public string? Rarity { get; set; }
    public string? WearCategory { get; set; }
    public string? CrateName { get; set; }
    public string? SkinName { get; set; }
    public long Timestamp { get; set; }
    
}
