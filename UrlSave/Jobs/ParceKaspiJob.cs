using Hangfire;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using UrlSave.Contexts;
using UrlSave.Entities;
using UrlSave.Services;

namespace UrlSave.Jobs
{
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
            _logger.LogInformation("StartKaspiParceJob:" + DateTime.Now);
            //todo we need to get only unique url links, to avoid double parsing the same product
            var links = _context.Links.ToList();
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

                Thread.Sleep(10000);
                
                var specElements = driver.FindElements(By.CssSelector("ul.short-specifications li.short-specifications__text"));
                var specName = new StringBuilder();
                foreach (var element in specElements)
                {
                    specName.Append(element.Text + " ");
                }
                var productName = driver.FindElement(By.CssSelector("h1.item__heading")).Text;

                var product = new Product()
                {
                    Name = productName,
                    Description = specName.ToString(),
                };
                var createdProduct = await _productService.AddAsync(product);

                link.Product = createdProduct;

                var minPrice = driver.FindElement(By.CssSelector("div.item__price-once")).Text;

                //todo check if last created price is changed only then create a new price
                var price = new Price()
                {
                    Value = ConvertPriceToLong(minPrice),
                    Product = createdProduct
                };
                _context.Prices.Add(price);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Price: {minPrice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                driver.Quit();
            }
        }

        long ConvertPriceToLong(string price)
        {
            string digits = new string(price.Where(char.IsDigit).ToArray());

            if (long.TryParse(digits, out long priceValue))
            {
                return priceValue;
            }
            else
            {
                Console.WriteLine("Не удалось конвертировать цену в число.");
                return 0;
            }
        }
    }
    

    
}
