namespace arcteryxScraper.Models;

public class SortOrder
{
    public static readonly List<string> OrderTypes = new() { "Price Ascending", "Price Descending", "Discount" };

    public int TypeSpecificComparator(string orderType)
    {
        return 0;
    }
}