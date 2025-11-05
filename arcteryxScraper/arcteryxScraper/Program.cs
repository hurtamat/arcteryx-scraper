using arcteryxScraper;
using arcteryxScraper.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(arcteryxScraper.Client._Imports).Assembly);

// Fetch HTML before starting the web server
var fetcher = new HtmlFetcher();
var htmlContent = await fetcher.FetchHtmlAsync("cz", "en", "mens");

// Parse the products from the HTML
var parser = new HtmlParser();
var products = parser.ParseProducts(htmlContent);
parser.DisplayProducts(products);

app.Run();