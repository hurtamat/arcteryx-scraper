using System.Text.RegularExpressions;

namespace arcteryxScraper.Parsers;

/// <summary>
/// Parses currencies with simple single-character symbols: € (EUR), $ (USD), £ (GBP)
/// Format examples: "€1,000.00", "$1,200.00", "£2,000.00"
/// </summary>
public class SimpleSymbolCurrencyParser : ICurrencyParser
{
    public (string currency, decimal price) ParsePriceWithCurrency(string priceString)
    {
        // Clean up HTML entities and extra whitespace
        var cleaned = priceString.Trim();

        // Extract currency symbol (single character at the start)
        var currencyMatch = Regex.Match(cleaned, @"^([€$£])");
        if (!currencyMatch.Success)
        {
            return ("", 0m);
        }

        var currency = currencyMatch.Groups[1].Value;

        // Extract numeric part
        var numericPart = cleaned.Substring(currency.Length).Trim();

        // Parse the price (period is decimal separator, comma is thousands separator)
        var price = ParsePrice(numericPart);

        return (currency, price);
    }

    private decimal ParsePrice(string priceString)
    {
        // For simple symbols (€, $, £), period is always the decimal separator
        // Remove comma (thousands separator) and any spaces
        var cleaned = priceString
            .Replace(",", "")
            .Replace(" ", "")
            .Trim();

        return decimal.Parse(cleaned);
    }
}
