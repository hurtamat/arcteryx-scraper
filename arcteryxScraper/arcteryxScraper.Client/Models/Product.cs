namespace arcteryxScraper.Client.Models;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal MinRangePrice { get; set; }
    public decimal? DiscountPrice { get; set; }
}
