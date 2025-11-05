using PuppeteerSharp;

namespace arcteryxScraper;

public class HtmlFetcher
{
    // URL components as variables
    public string Country { get; set; } = "cz";
    public string Language { get; set; } = "en";
    public string Category { get; set; } = "mens";

    // The fetched HTML content stored as a very long string
    public string HtmlContent { get; private set; } = string.Empty;

    public HtmlFetcher()
    {
    }

    /// <summary>
    /// Constructs the URL from the configured variables
    /// </summary>
    public string GetUrl()
    {
        return $"https://outlet.arcteryx.com/{Country}/{Language}/c/{Category}";
    }

    /// <summary>
    /// Fetches the HTML content using PuppeteerSharp headless browser to execute JavaScript and load dynamic content
    /// </summary>
    public async Task<string> FetchHtmlAsync()
    {
        var url = GetUrl();

        try
        {
            // Download Chromium if not already downloaded
            Console.WriteLine("Checking for Chromium browser...");
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            Console.WriteLine("Chromium ready!");

            // Launch browser in headless mode
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            // Create a new page
            await using var page = await browser.NewPageAsync();

            // Navigate to the URL and wait for DOM to be loaded
            Console.WriteLine($"Navigating to {url}...");
            await page.GoToAsync(url, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded },
                Timeout = 30000
            });

            Console.WriteLine("Page loaded, waiting for content to render...");

            // Wait for the product grid to load
            try
            {
                await page.WaitForSelectorAsync(".product-grid", new WaitForSelectorOptions
                {
                    Timeout = 10000
                });
                Console.WriteLine("Product grid found!");
            }
            catch
            {
                Console.WriteLine("Product grid not found within timeout, but continuing anyway...");
            }

            // Scroll the page to load dynamic content
            Console.WriteLine("Scrolling to load all dynamic content...");
            await ScrollPageAsync(page);

            // Get the full HTML content after JavaScript execution
            HtmlContent = await page.GetContentAsync();

            Console.WriteLine($"HTML content fetched successfully! ({HtmlContent.Length} characters)");

            return HtmlContent;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to fetch HTML from {url}", ex);
        }
    }

    /// <summary>
    /// Fetches HTML with custom country, language, and category parameters
    /// </summary>
    public async Task<string> FetchHtmlAsync(string country, string language, string category)
    {
        Country = country;
        Language = language;
        Category = category;

        return await FetchHtmlAsync();
    }

    /// <summary>
    /// Scrolls the page to trigger lazy loading of dynamic content
    /// Performs smooth scrolling in increments to ensure all lazy-loaded content is triggered
    /// </summary>
    private async Task ScrollPageAsync(IPage page, int maxScrollAttempts = 20, int scrollDelay = 1500)
    {
        int scrollAttempt = 0;
        int stableHeightCount = 0;
        int previousHeight = 0;

        while (scrollAttempt < maxScrollAttempts)
        {
            scrollAttempt++;

            // Get current scroll height
            var currentHeight = await page.EvaluateExpressionAsync<int>("document.body.scrollHeight");

            // Scroll down by a portion of the viewport height for smooth scrolling
            await page.EvaluateExpressionAsync(@"
                window.scrollBy({
                    top: window.innerHeight * 0.8,
                    behavior: 'smooth'
                });
            ");

            Console.WriteLine($"Scroll {scrollAttempt} - Current page height: {currentHeight}px");

            // Wait for content to load
            await Task.Delay(scrollDelay);

            // Also scroll to absolute bottom to trigger any remaining content
            await page.EvaluateExpressionAsync("window.scrollTo(0, document.body.scrollHeight)");
            await Task.Delay(500);

            // Check if page height has changed
            var newHeight = await page.EvaluateExpressionAsync<int>("document.body.scrollHeight");

            if (newHeight == previousHeight)
            {
                stableHeightCount++;
                Console.WriteLine($"Page height stable ({stableHeightCount}/3)");

                // If height hasn't changed for 3 consecutive checks, we've reached the bottom
                if (stableHeightCount >= 3)
                {
                    Console.WriteLine("Reached bottom of page - no new content loaded after 3 checks.");
                    break;
                }
            }
            else
            {
                stableHeightCount = 0; // Reset counter if page grew
                Console.WriteLine($"New content loaded! Page grew from {previousHeight}px to {newHeight}px");
            }

            previousHeight = newHeight;
        }

        // Final scroll to absolute bottom
        await page.EvaluateExpressionAsync("window.scrollTo(0, document.body.scrollHeight)");
        await Task.Delay(1000);

        Console.WriteLine("Scrolling completed - entire page loaded!");
    }
}
