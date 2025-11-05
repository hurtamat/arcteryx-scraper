using System.Text.RegularExpressions;

namespace arcteryxScraper;

public class HtmlParser
{
    /// <summary>
    /// Parses the HTML content and extracts all products with their pricing information
    /// </summary>
    public List<Product> ParseProducts(string htmlContent)
    {
        var products = new List<Product>();

        // Pattern to match product tiles - we look for the qa--grid-product-tile sections
        var productTilePattern = @"qa--grid-product-tile.*?(?=qa--grid-product-tile|$)";
        var productTileMatches = Regex.Matches(htmlContent, productTilePattern, RegexOptions.Singleline);

        Console.WriteLine($"Found {productTileMatches.Count} product tiles");

        foreach (Match tileMatch in productTileMatches)
        {
            var tileHtml = tileMatch.Value;

            try
            {
                var product = ExtractProductFromTile(tileHtml);
                if (product != null)
                {
                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing product tile: {ex.Message}");
            }
        }

        return products;
    }

    /// <summary>
    /// Extracts product information from a single product tile HTML
    /// </summary>
    private Product? ExtractProductFromTile(string tileHtml)
    {
        // Extract product name - pattern: data-component="body3">Product Name</div>
        var namePattern = @"data-component=""body3"">([^<]+)</div>";
        var nameMatch = Regex.Match(tileHtml, namePattern);

        if (!nameMatch.Success)
        {
            return null; // Skip if we can't find a product name
        }

        var product = new Product
        {
            Name = nameMatch.Groups[1].Value.Trim()
        };

        // Extract product URL - pattern: qa--product-tile__link" href="/cz/en/shop/..."
        var urlPattern = @"qa--product-tile__link""\s+href=""([^""]+)""";
        var urlMatch = Regex.Match(tileHtml, urlPattern);

        if (urlMatch.Success)
        {
            product.Url = urlMatch.Groups[1].Value.Trim();
        }

        // Extract original price - pattern: qa--product-tile__original-price">€XXX.XX</span>
        var originalPricePattern = @"qa--product-tile__original-price"">€([\d,]+\.\d{2})</span>";
        var originalPriceMatch = Regex.Match(tileHtml, originalPricePattern);

        if (originalPriceMatch.Success)
        {
            product.OriginalPrice = ParsePrice(originalPriceMatch.Groups[1].Value);
        }

        // Extract min range price - pattern: qa--product-tile__minRange-price">€XXX.XX</span>
        var minRangePricePattern = @"qa--product-tile__minRange-price"">€([\d,]+\.\d{2})</span>";
        var minRangePriceMatch = Regex.Match(tileHtml, minRangePricePattern);

        if (minRangePriceMatch.Success)
        {
            product.MinRangePrice = ParsePrice(minRangePriceMatch.Groups[1].Value);
        }

        // Extract discount price (optional) - pattern: qa--product-tile__discount-price">€XXX.XX</span>
        var discountPricePattern = @"qa--product-tile__discount-price"">€([\d,]+\.\d{2})</span>";
        var discountPriceMatch = Regex.Match(tileHtml, discountPricePattern);

        if (discountPriceMatch.Success)
        {
            product.DiscountPrice = ParsePrice(discountPriceMatch.Groups[1].Value);
        }

        return product;
    }

    /// <summary>
    /// Parses a price string (e.g., "850.00" or "1,250.50") to a decimal
    /// </summary>
    private decimal ParsePrice(string priceString)
    {
        // Remove commas and parse
        var cleanPrice = priceString.Replace(",", "");
        return decimal.Parse(cleanPrice);
    }

    /// <summary>
    /// Displays all products in a formatted way
    /// </summary>
    public void DisplayProducts(List<Product> products)
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
