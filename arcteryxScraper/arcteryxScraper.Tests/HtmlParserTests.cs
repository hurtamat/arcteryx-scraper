namespace arcteryxScraper.Tests;

public class HtmlParserTests
{
    private readonly HtmlParser _parser;

    public HtmlParserTests()
    {
        _parser = new HtmlParser();
    }

    [Fact]
    public void ParsePriceWithCurrency_SwissFrancWithApostrophe_ParsesCorrectly()
    {
        // Arrange
        var input = "CHF&nbsp;1'099.00";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("CHF", currency);
        Assert.Equal(1099.00m, price);
    }

    [Fact]
    public void ParsePriceWithCurrency_NorwegianKroneWithSpaceAndCommaDecimal_ParsesCorrectly()
    {
        // Arrange
        var input = "6&nbsp;599,40&nbsp;kr";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("kr", currency);
        Assert.Equal(6599.40m, price);
    }

    [Fact]
    public void ParsePriceWithCurrency_EuroWithCommaThousandSeparator_ParsesCorrectly()
    {
        // Arrange
        var input = "€1,000.00";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("€", currency);
        Assert.Equal(1000.00m, price);
    }

    [Fact]
    public void ParsePriceWithCurrency_DollarWithCommaThousandSeparator_ParsesCorrectly()
    {
        // Arrange
        var input = "$1,200.00";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("$", currency);
        Assert.Equal(1200.00m, price);
    }

    [Fact]
    public void ParsePriceWithCurrency_PoundWithCommaThousandSeparator_ParsesCorrectly()
    {
        // Arrange
        var input = "£2,000.00";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("£", currency);
        Assert.Equal(2000.00m, price);
    }

    [Fact]
    public void ParsePriceWithCurrency_PolishZlotyWithCommaThousandSeparator_ParsesCorrectly()
    {
        // Arrange
        var input = "PLN&nbsp;10,499.00";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("PLN", currency);
        Assert.Equal(10499.00m, price);
    }

    [Fact]
    public void ParsePriceWithCurrency_LargeKroneWithSpaceAndCommaDecimal_ParsesCorrectly()
    {
        // Arrange
        var input = "23&nbsp;999,00&nbsp;kr";

        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal("kr", currency);
        Assert.Equal(23999.00m, price);
    }

    [Theory]
    [InlineData("CHF&nbsp;1'099.00", "CHF", 1099.00)]
    [InlineData("6&nbsp;599,40&nbsp;kr", "kr", 6599.40)]
    [InlineData("€1,000.00", "€", 1000.00)]
    [InlineData("$1,200.00", "$", 1200.00)]
    [InlineData("£2,000.00", "£", 2000.00)]
    [InlineData("PLN&nbsp;10,499.00", "PLN", 10499.00)]
    [InlineData("23&nbsp;999,00&nbsp;kr", "kr", 23999.00)]
    public void ParsePriceWithCurrency_VariousFormats_ParsesCorrectly(string input, string expectedCurrency, decimal expectedPrice)
    {
        // Act
        var (currency, price) = _parser.ParsePriceWithCurrency(input);

        // Assert
        Assert.Equal(expectedCurrency, currency);
        Assert.Equal(expectedPrice, price);
    }
}
