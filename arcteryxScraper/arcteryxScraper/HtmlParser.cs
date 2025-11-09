using System.Text.RegularExpressions;
using arcteryxScraper.Models;
using arcteryxScraper.Parsers;

namespace arcteryxScraper;

public class HtmlParser
{
    /// <summary>
    /// Parses the HTML content and extracts all products with their pricing information
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse</param>
    /// <param name="country">The country to use for currency parsing</param>
    public List<Product> ParseProducts(string htmlContent, InputCountry country)
    {
        var products = new List<Product>();
        var currencyParser = CurrencyParserFactory.CreateParser(country);

        // Pattern to match product tiles - we look for the qa--grid-product-tile sections
        var productTilePattern = @"qa--grid-product-tile.*?(?=qa--grid-product-tile|$)";
        var productTileMatches = Regex.Matches(htmlContent, productTilePattern, RegexOptions.Singleline);

        Console.WriteLine($"Found {productTileMatches.Count} product tiles");

        foreach (Match tileMatch in productTileMatches)
        {
            var tileHtml = tileMatch.Value;

            try
            {
                var product = ExtractProductFromTile(tileHtml, currencyParser);
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
    /// <param name="tileHtml">The HTML content of the product tile</param>
    /// <param name="currencyParser">The currency parser to use for this country</param>
    private Product? ExtractProductFromTile(string tileHtml, ICurrencyParser currencyParser)
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
            var relativeUrl = urlMatch.Groups[1].Value.Trim();
            // Convert relative URL to absolute URL with outlet.arcteryx.com domain
            product.Url = relativeUrl.StartsWith("http")
                ? relativeUrl
                : $"https://outlet.arcteryx.com{relativeUrl}";
        }

        // Extract original price - pattern: qa--product-tile__original-price">€XXX.XX</span> or PLN&nbsp;XXX.XX or $XXX.XX
        var originalPricePattern = @"qa--product-tile__original-price"">([^<]+)</span>";
        var originalPriceMatch = Regex.Match(tileHtml, originalPricePattern);

        if (originalPriceMatch.Success)
        {
            var (currency, price) = currencyParser.ParsePriceWithCurrency(originalPriceMatch.Groups[1].Value);
            product.Currency = currency;
            product.OriginalPrice = price;
        }

        // Extract min range price - pattern: qa--product-tile__minRange-price">€XXX.XX</span> or PLN&nbsp;XXX.XX or $XXX.XX
        var minRangePricePattern = @"qa--product-tile__minRange-price"">([^<]+)</span>";
        var minRangePriceMatch = Regex.Match(tileHtml, minRangePricePattern);

        if (minRangePriceMatch.Success)
        {
            var (currency, price) = currencyParser.ParsePriceWithCurrency(minRangePriceMatch.Groups[1].Value);
            if (string.IsNullOrEmpty(product.Currency))
            {
                product.Currency = currency;
            }
            product.MinRangePrice = price;
        }

        // Extract discount price (optional) - pattern: qa--product-tile__discount-price">€XXX.XX</span> or PLN&nbsp;XXX.XX or $XXX.XX
        var discountPricePattern = @"qa--product-tile__discount-price"">([^<]+)</span>";
        var discountPriceMatch = Regex.Match(tileHtml, discountPricePattern);

        if (discountPriceMatch.Success)
        {
            var (currency, price) = currencyParser.ParsePriceWithCurrency(discountPriceMatch.Groups[1].Value);
            if (string.IsNullOrEmpty(product.Currency))
            {
                product.Currency = currency;
            }
            product.DiscountPrice = price;
        }

        return product;
    }
}
