namespace Cs2CaseOpener.Contracts;

// Request for a single opening, used within the batch
public record CrateOpeningRequest
{
    public string CrateId { get; set; } = null!;
    public string SkinId { get; set; } = null!;
    public string Rarity { get; set; } = null!;
    public string WearCategory { get; set; } = null!;
    public string CrateName { get; set; } = null!;
    public string SkinName { get; set; } = null!;
    public long Timestamp { get; set; }
}