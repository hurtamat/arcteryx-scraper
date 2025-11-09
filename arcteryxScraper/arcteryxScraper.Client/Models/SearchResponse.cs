namespace arcteryxScraper.Client.Models;

public class SearchResponse
{
    public Catalog? Catalog { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success => string.IsNullOrEmpty(ErrorMessage);
}
