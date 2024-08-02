using Hangfire;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using UrlSave.Contexts;
using UrlSave.Entities;
using UrlSave.Services;

namespace UrlSave.Jobs;

public class ParceKaspiJob
{
    private readonly ILogger<ParceKaspiJob> _logger;
    private readonly LinkContext _context;
    private readonly ProductService _productService;
    public ParceKaspiJob(ILogger<ParceKaspiJob> logger, LinkContext context, ProductService productService)
    {
        _logger = logger;
        _context = context;
        _productService = productService;
    }

    [JobDisplayName("KaspiJob")]
    public async Task Execute()
    {
        //todo we need to get only unique url links, to avoid double parsing the same product
        var links = await _context.Links.ToListAsync();
        foreach (var link in links)
        {
           await ParcerCode(link);
        }
    }

    private async Task ParcerCode(Link link)
    {
        IWebDriver driver = new ChromeDriver();
        try
        {
            driver.Navigate().GoToUrl(link.Url);

            Random random = new();
            int randomNumber = random.Next(10000, 20001);
            Thread.Sleep(randomNumber);
            
            var specElements = driver.FindElements(By.CssSelector("ul.short-specifications li.short-specifications__text"));
            var specName = new StringBuilder();
            foreach (var element in specElements)
            {
                specName.Append(element.Text + " ");
            }
            var productName = driver.FindElement(By.CssSelector("h1.item__heading")).Text;

            var product = new Product(productName, specName.ToString());
            var createdProduct = await _productService.AddAsync(product);
            link.Product = createdProduct;

            var minPrice = driver.FindElement(By.CssSelector("div.item__price-once")).Text;

            var priceLong = ConvertPriceToLong(minPrice);
            await CreatePrice(priceLong, createdProduct);

            _logger.LogInformation("Price: {minPrice}", minPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        finally
        {
            driver.Quit();
        }
    }

    private async Task CreatePrice(long price, Product product)
    {
        var lastPrice = await _context.Prices
            .Where(x => x.ProductId == product.Id)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync();

        if (lastPrice?.Value != price)
        {
            var newPrice = new Price
            {
                Value = price,
                Product = product
            };
            _context.Prices.Add(newPrice);
            await _context.SaveChangesAsync();
        }
    }

    private long ConvertPriceToLong(string price)
    {
        string digits = new(price.Where(char.IsDigit).ToArray());

        if (long.TryParse(digits, out long priceValue))
        {
            return priceValue;
        }
        else
        {
            _logger.LogError("Cannot convert price to long.");
            return 0;
        }
    }
}

