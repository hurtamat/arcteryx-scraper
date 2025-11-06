namespace arcteryxScraper;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal MinRangePrice { get; set; }
    public decimal? DiscountPrice { get; set; }

    public override string ToString()
    {
        var discountInfo = DiscountPrice.HasValue ? $" - €{DiscountPrice.Value:F2}" : "";
        return $"{Name}\n  URL: {Url}\n  Original: €{OriginalPrice:F2}\n  Min Range: €{MinRangePrice:F2}{discountInfo}";
    }
}
