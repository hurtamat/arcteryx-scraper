using arcteryxScraper.Models;

namespace arcteryxScraper.Parsers;

/// <summary>
/// Factory for creating the appropriate currency parser based on country
/// </summary>
public static class CurrencyParserFactory
{
    /// <summary>
    /// Creates the appropriate currency parser for the given country
    /// </summary>
    /// <param name="country">The country code</param>
    /// <returns>The appropriate currency parser implementation</returns>
    public static ICurrencyParser CreateParser(InputCountry country)
    {
        return country switch
        {
            InputCountry.At => new SimpleSymbolCurrencyParser(),
            InputCountry.Be => new SimpleSymbolCurrencyParser(),
            InputCountry.Fi => new SimpleSymbolCurrencyParser(),
            InputCountry.Fr => new SimpleSymbolCurrencyParser(),
            InputCountry.De => new SimpleSymbolCurrencyParser(),
            InputCountry.Ie => new SimpleSymbolCurrencyParser(),
            InputCountry.It => new SimpleSymbolCurrencyParser(),
            InputCountry.Es => new SimpleSymbolCurrencyParser(),
            InputCountry.Nl => new SimpleSymbolCurrencyParser(),
            InputCountry.Ca => new SimpleSymbolCurrencyParser(),
            InputCountry.Us => new SimpleSymbolCurrencyParser(),
            InputCountry.Gb => new SimpleSymbolCurrencyParser(),
            InputCountry.Cz => new SimpleSymbolCurrencyParser(),
            InputCountry.Ch => new ChfCurrencyParser(),
            InputCountry.No => new NokCurrencyParser(),
            InputCountry.Pl => new PlnCurrencyParser(),
            InputCountry.Dk => new ScandinavianKroneCurrencyParser(),
            InputCountry.Se => new ScandinavianKroneCurrencyParser(),
            

            _ => throw new ArgumentException($"Unsupported country: {country}", nameof(country))
        };
    }
}
