using arcteryxScraper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace arcteryxScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly HtmlFetcher _fetcher;
    private readonly HtmlParser _parser;
    private readonly ILogger<ProductsController> _logger;
    private readonly IMemoryCache _cache;

    public ProductsController(
        HtmlFetcher fetcher,
        HtmlParser parser,
        ILogger<ProductsController> logger,
        IMemoryCache cache)
    {
        _fetcher = fetcher;
        _parser = parser;
        _logger = logger;
        _cache = cache;
    }

    [HttpPost("search")]
    public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest request)
    {
        try
        {
            // Create cache key from country and gender
            var cacheKey = $"catalog_{request.Country}_{request.Gender}";

            // Try to get from cache
            if (_cache.TryGetValue<Catalog>(cacheKey, out var cachedCatalog))
            {
                _logger.LogInformation("Returning cached catalog for {Country} - {Gender}", request.Country, request.Gender);
                return Ok(new SearchResponse
                {
                    Catalog = cachedCatalog
                });
            }

            _logger.LogInformation("Cache miss. Fetching products for {Country} - {Gender}", request.Country, request.Gender);

            // Convert enum to lowercase string for API
            string country = request.Country.ToString().ToLower();
            string gender = request.Gender.ToString().ToLower();

            // Fetch HTML content
            var htmlContent = await _fetcher.FetchHtmlAsync(country, "en", gender);

            // Parse products using the appropriate currency parser (returns Catalog)
            var catalog = _parser.ParseProducts(htmlContent, request.Country);

            _logger.LogInformation("Found {Count} products for {Country} - {Gender}",
                catalog.products.Count, request.Country, request.Gender);

            // Cache for 2 hours
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(2));

            _cache.Set(cacheKey, catalog, cacheOptions);
            _logger.LogInformation("Cached catalog for {Country} - {Gender} for 2 hours", request.Country, request.Gender);

            return Ok(new SearchResponse
            {
                Catalog = catalog
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            return Ok(new SearchResponse
            {
                ErrorMessage = ex.Message
            });
        }
    }
}
