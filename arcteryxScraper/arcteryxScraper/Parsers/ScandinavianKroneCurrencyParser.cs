using System.Text.RegularExpressions;

namespace arcteryxScraper.Parsers;

/// <summary>
/// Parses Scandinavian Krone currencies (SEK, DKK, or generic "kr")
/// Format examples: "6 599,40 kr", "23 999,00 kr", "6&nbsp;599,40&nbsp;kr"
/// Uses space as thousands separator and comma as decimal separator
/// Currency code appears at the END of the string
/// </summary>
public class ScandinavianKroneCurrencyParser : ICurrencyParser
{
    public (string currency, decimal price) ParsePriceWithCurrency(string priceString)
    {
        // Clean up HTML entities and extra whitespace
        var cleaned = priceString
            .Replace("&nbsp;", " ")
            .Replace("&#160;", " ")
            .Trim();

        // Extract "kr" currency code at the end
        var currencyMatch = Regex.Match(cleaned, @"(kr|SEK|DKK)$", RegexOptions.IgnoreCase);
        if (!currencyMatch.Success)
        {
            return ("", 0m);
        }

        var currency = currencyMatch.Groups[1].Value;

        // Extract numeric part (everything before the currency code)
        var numericPart = cleaned.Substring(0, cleaned.Length - currency.Length).Trim();

        // Parse the price (comma is decimal separator, space is thousands separator)
        var price = ParsePrice(numericPart);

        return (currency, price);
    }

    private decimal ParsePrice(string priceString)
    {
        // Scandinavian Krone uses space as thousands separator and comma as decimal separator
        // Remove spaces and replace comma with period for parsing
        var cleaned = priceString
            .Replace(" ", "")
            .Replace(",", ".")
            .Trim();

        return decimal.Parse(cleaned);
    }
}
