using System.Linq;

namespace arcteryxScraper.Models;

public class Catalog
{
    public List<Product> Products { get; set; }

    public Catalog(List<Product> products)
    {
        Products = products;
    }

    public void SortProducts(SortOrderType sortOrder)
    {
        Products = sortOrder switch
        {
            SortOrderType.PriceAscending => Products.OrderBy(p => p.MinRangePrice).ToList(),
            SortOrderType.PriceDescending => Products.OrderByDescending(p => p.MinRangePrice).ToList(),
            SortOrderType.Discount => Products.OrderByDescending(p =>
                p.DiscountPrice.HasValue ? (p.OriginalPrice - p.DiscountPrice.Value) : 0).ToList(),
            _ => Products
        };
    }
}