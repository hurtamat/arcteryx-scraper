using System.Text.RegularExpressions;

namespace arcteryxScraper.Parsers;

/// <summary>
/// Parses Swiss Franc (CHF) currency
/// Format examples: "CHF 1'099.00", "CHF&nbsp;1'099.00"
/// Uses apostrophe as thousands separator and period as decimal separator
/// </summary>
public class ChfCurrencyParser : ICurrencyParser
{
    public (string currency, decimal price) ParsePriceWithCurrency(string priceString)
    {
        // Clean up HTML entities and extra whitespace
        var cleaned = priceString
            .Replace("&nbsp;", " ")
            .Replace("&#160;", " ")
            .Trim();

        // Extract CHF currency code
        var currencyMatch = Regex.Match(cleaned, @"^(CHF)", RegexOptions.IgnoreCase);
        if (!currencyMatch.Success)
        {
            return ("", 0m);
        }

        var currency = currencyMatch.Groups[1].Value;

        // Extract numeric part (everything after CHF)
        var numericPart = cleaned.Substring(currency.Length).Trim();

        // Parse the price (apostrophe is thousands separator, period is decimal separator)
        var price = ParsePrice(numericPart);

        return (currency, price);
    }

    private decimal ParsePrice(string priceString)
    {
        // CHF uses apostrophe as thousands separator and period as decimal separator
        // Remove apostrophe (thousands separator) and any spaces
        var cleaned = priceString
            .Replace("'", "")
            .Replace(" ", "")
            .Trim();

        return decimal.Parse(cleaned);
    }
}
