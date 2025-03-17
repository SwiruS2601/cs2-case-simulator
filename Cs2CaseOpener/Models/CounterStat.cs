namespace Cs2CaseOpener.Models;

public class CounterStat
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public long Value { get; set; }
    public DateTime LastUpdated { get; set; }
}