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
            var (currency, price) = ParsePriceWithCurrency(originalPriceMatch.Groups[1].Value);
            product.Currency = currency;
            product.OriginalPrice = price;
        }

        // Extract min range price - pattern: qa--product-tile__minRange-price">€XXX.XX</span> or PLN&nbsp;XXX.XX or $XXX.XX
        var minRangePricePattern = @"qa--product-tile__minRange-price"">([^<]+)</span>";
        var minRangePriceMatch = Regex.Match(tileHtml, minRangePricePattern);

        if (minRangePriceMatch.Success)
        {
            var (currency, price) = ParsePriceWithCurrency(minRangePriceMatch.Groups[1].Value);
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
            var (currency, price) = ParsePriceWithCurrency(discountPriceMatch.Groups[1].Value);
            if (string.IsNullOrEmpty(product.Currency))
            {
                product.Currency = currency;
            }
            product.DiscountPrice = price;
        }

        return product;
    }

    /// <summary>
    /// Parses a price string handling various formats:
    /// - US/UK: "1,234.56"
    /// - Europe: "1.234,56" or "1 234,56"
    /// - Swiss: "1'199.00"
    /// </summary>
    internal decimal ParsePrice(string priceString)
    {
        var cleaned = priceString.Trim();

        // Determine decimal separator by checking last 3 characters
        // If format is X,XX or X.XX at the end, that's the decimal separator
        bool usesCommaAsDecimal = Regex.IsMatch(cleaned, @",\d{2}$");
        bool usesPeriodAsDecimal = Regex.IsMatch(cleaned, @"\.\d{2}$");

        if (usesCommaAsDecimal)
        {
            // European format: comma is decimal, everything else is thousand separator
            cleaned = cleaned.Replace(".", "").Replace("'", "").Replace(" ", "");
            cleaned = cleaned.Replace(",", ".");
        }
        else if (usesPeriodAsDecimal || cleaned.Contains('.'))
        {
            // US/UK/Swiss format: period is decimal, everything else is thousand separator
            cleaned = cleaned.Replace(",", "").Replace("'", "").Replace(" ", "");
        }
        else
        {
            // No decimal separator found, just remove thousand separators
            cleaned = cleaned.Replace(",", "").Replace("'", "").Replace(" ", "");
        }

        return decimal.Parse(cleaned);
    }

    /// <summary>
    /// Parses a price string with currency (e.g., "€850.00", "PLN&nbsp;2,309.30", "$84.00", "10 999,00 kr")
    /// Returns a tuple of (currency, price)
    /// </summary>
    internal (string currency, decimal price) ParsePriceWithCurrency(string priceString)
    {
        // Clean up HTML entities and extra whitespace
        var cleaned = priceString
            .Replace("&nbsp;", "")
            .Replace("&#160;", "")
            .Trim();

        // Pattern to extract currency and numeric value
        // Matches: currency symbols/codes followed by number with optional comma and decimals
        var pattern = @"^([^\d]+?)\s*([\d,]+\.?\d*)$";
        var match = Regex.Match(cleaned, pattern);

        if (match.Success)
        {
            var currency = match.Groups[1].Value.Trim();
            var numericValue = match.Groups[2].Value;
            var price = ParsePrice(numericValue);
            return (currency, price);
        }

        // Fallback: try to extract just numbers if pattern doesn't match
        var numberPattern = @"([\d,]+\.?\d*)";
        var numberMatch = Regex.Match(cleaned, numberPattern);

        if (numberMatch.Success)
        {
            var price = ParsePrice(numberMatch.Groups[1].Value);
            // Try to extract anything before the number as currency
            var currencyPattern = @"^([^\d]+)";
            var currencyMatch = Regex.Match(cleaned, currencyPattern);
            var currency = currencyMatch.Success ? currencyMatch.Groups[1].Value.Trim() : "";
            return (currency, price);
        }

        // If all else fails, return empty currency and 0 price
        return ("", 0m);
    }
}
