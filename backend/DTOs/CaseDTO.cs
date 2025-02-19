namespace Cs2CaseOpener.DTOs;
public class CaseDTO
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? FirstSaleDate { get; set; }
    public string? Market_Hash_Name { get; set; }
    public bool? Rental { get; set; }
    public string? Image { get; set; }
    public string? Model_Player { get; set; }
}