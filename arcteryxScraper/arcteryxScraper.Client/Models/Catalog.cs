namespace arcteryxScraper.Client.Models;

public class Catalog
{
    public List<Product> products { get; set; }
    public string Currency { get; set; } = string.Empty;

    public Catalog(List<Product> products, string currency)
    {
        this.products = products;
        this.Currency = currency;
    }

    public void SortProducts(SortOrderType sortOrder)
    {
        products = sortOrder switch
        {
            SortOrderType.PriceAscending => products.OrderBy(p => p.MinRangePrice).ToList(),
            SortOrderType.PriceDescending => products.OrderByDescending(p => p.MinRangePrice).ToList(),
            SortOrderType.DiscountByAmount => products.OrderByDescending(p => p.OriginalPrice - p.MinRangePrice).ToList(),
            SortOrderType.DiscountByPercentage => products.OrderByDescending(p => (p.OriginalPrice - p.MinRangePrice)/p.OriginalPrice*100).ToList(),
            _ => products
        };
    }
}
