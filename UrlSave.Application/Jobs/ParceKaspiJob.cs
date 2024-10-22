using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using UrlSave.Application.Extensions;
using UrlSave.Application.Interfaces;
using UrlSave.Application.Services;
using UrlSave.Domain.Entities;

namespace UrlSave.Application.Jobs;

public class ParceKaspiJob : IParceKaspiJob
{
    private readonly ILogger<ParceKaspiJob> _logger;
    private readonly ILinkContext _context;
    private readonly ProductService _productService;
    public ParceKaspiJob(ILogger<ParceKaspiJob> logger, ILinkContext context, ProductService productService)
    {
        _logger = logger;
        _context = context;
        _productService = productService;
    }

    [JobDisplayName("KaspiJob")]
    public async Task ParseKaspiLinks()
    {
        //todo we need to get only unique url links, to avoid double parsing the same product
        var links = await _context.Links.ToListAsync();
        foreach (var link in links)
        {
            await ParcerCode(link);
        }
    }

    public async Task ParcerCode(Link link)
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

            var parcedMinPrice = driver.FindElement(By.CssSelector("div.item__price-once")).Text;

            await AddNewPrice(parcedMinPrice.ToLong(), createdProduct);

            _logger.LogInformation("Price: {minPrice}", parcedMinPrice);
            await _context.SaveChangesAsync();
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

    public async Task AddNewPrice(long price, Product product)
    {
        var lastPrice = await _context.Prices
            .AsNoTracking()
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
        }
    }
}
