namespace arcteryxScraper.Parsers;

public interface ICurrencyParser
{
    /// <summary>
    /// Parses a price string and extracts the currency code and decimal value.
    /// </summary>
    /// <param name="priceString">The price string to parse (e.g., "€1,000.00", "NOK 5,249.30")</param>
    /// <returns>A tuple containing the currency code and the parsed price</returns>
    (string currency, decimal price) ParsePriceWithCurrency(string priceString);
}
