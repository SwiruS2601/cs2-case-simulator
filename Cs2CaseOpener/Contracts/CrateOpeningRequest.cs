namespace Cs2CaseOpener.Contracts;

public record CrateOpeningBatchRequest
{
    public required List<CrateOpeningRequest> Openings { get; set; }
    public string? ClientId { get; set; }
}
