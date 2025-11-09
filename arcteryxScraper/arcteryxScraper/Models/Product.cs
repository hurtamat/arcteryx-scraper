namespace arcteryxScraper;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal MinRangePrice { get; set; }
    public decimal? DiscountPrice { get; set; }

    public override string ToString()
    {
        var discountInfo = DiscountPrice.HasValue ? $" - {Currency}{DiscountPrice.Value:F2}" : "";
        return $"{Name}\n  URL: {Url}\n  Original: {Currency}{OriginalPrice:F2}\n  Min Range: {Currency}{MinRangePrice:F2}{discountInfo}";
    }
}
