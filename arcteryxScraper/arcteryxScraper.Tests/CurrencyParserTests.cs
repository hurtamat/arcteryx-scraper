using arcteryxScraper.Models;
using arcteryxScraper.Parsers;
using Xunit;

namespace arcteryxScraper.Tests;

public class CurrencyParserTests
{
    #region SimpleSymbolCurrencyParser Tests

    [Theory]
    [InlineData("€1,000.00", "€", 1000.00)]
    [InlineData("$1,200.00", "$", 1200.00)]
    [InlineData("£2,000.00", "£", 2000.00)]
    public void SimpleSymbolCurrencyParser_ValidFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice)
    {
        // Arrange
        var parser = new SimpleSymbolCurrencyParser();

        // Act
        var (currency, price) = parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }

    #endregion

    #region NokCurrencyParser Tests

    [Theory]
    [InlineData("NOK&nbsp;5,249.30", "NOK", 5249.30)]
    public void NokCurrencyParser_ValidFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice)
    {
        // Arrange
        var parser = new NokCurrencyParser();

        // Act
        var (currency, price) = parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }

    #endregion

    #region PlnCurrencyParser Tests

    [Theory]
    [InlineData("PLN&nbsp;10,499.00", "PLN", 10499.00)]
    public void PlnCurrencyParser_ValidFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice)
    {
        // Arrange
        var parser = new PlnCurrencyParser();

        // Act
        var (currency, price) = parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }

    #endregion

    #region ChfCurrencyParser Tests

    [Theory]
    [InlineData("CHF&nbsp;1'099.00", "CHF", 1099.00)]
    public void ChfCurrencyParser_ValidFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice)
    {
        // Arrange
        var parser = new ChfCurrencyParser();

        // Act
        var (currency, price) = parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }

    #endregion

    #region ScandinavianKroneCurrencyParser Tests

    [Theory]
    [InlineData("6&nbsp;599,40&nbsp;kr", "kr", 6599.40)]
    [InlineData("23&nbsp;999,00&nbsp;kr", "kr", 23999.00)]
    public void ScandinavianKroneCurrencyParser_ValidFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice)
    {
        // Arrange
        var parser = new ScandinavianKroneCurrencyParser();

        // Act
        var (currency, price) = parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }

    #endregion

    #region Integration Tests (matching original HtmlParser tests)

    [Theory]
    [InlineData("CHF&nbsp;1'099.00", "CHF", 1099.00, InputCountry.Ch)]
    [InlineData("6&nbsp;599,40&nbsp;kr", "kr", 6599.40, InputCountry.Se)]
    [InlineData("€1,000.00", "€", 1000.00, InputCountry.De)]
    [InlineData("$1,200.00", "$", 1200.00, InputCountry.Us)]
    [InlineData("£2,000.00", "£", 2000.00, InputCountry.Gb)]
    [InlineData("PLN&nbsp;10,499.00", "PLN", 10499.00, InputCountry.Pl)]
    [InlineData("23&nbsp;999,00&nbsp;kr", "kr", 23999.00, InputCountry.Dk)]
    [InlineData("NOK&nbsp;5,249.30", "NOK", 5249.30, InputCountry.No)]
    public void CurrencyParserFactory_VariousFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice, InputCountry country)
    {
        // Arrange
        var parser = CurrencyParserFactory.CreateParser(country);

        // Act
        var (currency, price) = parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }

    #endregion
}
