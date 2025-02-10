namespace Cs2CaseOpener.Models;

public class TimeframePrices
{
    public string? Sold { get; set; }
    public double? Median { get; set; }
    public double? Average { get; set; }
    public double? Lowest_Price { get; set; }
    public double? Highest_Price { get; set; }
    public string? Standard_Deviation { get; set; }
}

public class WearPrices
{
    public TimeframePrices? Seven_Days { get; set; }
    public TimeframePrices? Thirty_Days { get; set; }
    public TimeframePrices? Twenty_Four_Hours { get; set; }
    public TimeframePrices? All_Time { get; set; }
}

public class SkinPriceDetails
{
    public WearPrices? Well_Worn { get; set; }
    public WearPrices? Factory_New { get; set; }
    public WearPrices? Field_Tested { get; set; }
    public WearPrices? Minimal_Wear { get; set; }
}
