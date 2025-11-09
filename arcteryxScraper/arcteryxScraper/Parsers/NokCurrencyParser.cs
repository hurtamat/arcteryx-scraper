using System.Text.RegularExpressions;

namespace arcteryxScraper.Parsers;

/// <summary>
/// Parses Norwegian Krone (NOK) currency
/// Format examples: "NOK 5,249.30", "NOK&nbsp;5,249.30"
/// Uses period as decimal separator and comma as thousands separator
/// </summary>
public class NokCurrencyParser : ICurrencyParser
{
    public (string currency, decimal price) ParsePriceWithCurrency(string priceString)
    {
        // Clean up HTML entities and extra whitespace
        var cleaned = priceString
            .Replace("&nbsp;", " ")
            .Replace("&#160;", " ")
            .Trim();

        // Extract NOK currency code
        var currencyMatch = Regex.Match(cleaned, @"^(NOK)", RegexOptions.IgnoreCase);
        if (!currencyMatch.Success)
        {
            return ("", 0m);
        }

        var currency = currencyMatch.Groups[1].Value;

        // Extract numeric part (everything after NOK)
        var numericPart = cleaned.Substring(currency.Length).Trim();

        // Parse the price (period is decimal separator, comma is thousands separator)
        var price = ParsePrice(numericPart);

        return (currency, price);
    }

    private decimal ParsePrice(string priceString)
    {
        // NOK uses period as decimal separator and comma as thousands separator
        // Remove comma (thousands separator) and any spaces
        var cleaned = priceString
            .Replace(",", "")
            .Replace(" ", "")
            .Trim();

        return decimal.Parse(cleaned);
    }
}
