using System.Linq;

namespace arcteryxScraper.Models;

public class Catalog
{
    public List<Product> products { get; set; }

    public Catalog(List<Product> products)
    {
        this.products = products;
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
    
    public void DisplayProducts()
    {
        Console.WriteLine($"\n{'='} PARSED PRODUCTS {'='}\n");
        Console.WriteLine($"Total products found: {products.Count}\n");

        foreach (var product in products)
        {
            Console.WriteLine(product.ToString());
            Console.WriteLine();
        }
    }
}