using Hangfire;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using UrlSave.Contexts;
using UrlSave.Entities;
using UrlSave.Models;

namespace UrlSave.Jobs
{
    public class ParceKaspiJob
    {
        private readonly ILogger<ParceKaspiJob> _logger;
        private readonly LinkContext _context;

        public ParceKaspiJob(ILogger<ParceKaspiJob> logger, LinkContext context)
        {
            _logger = logger;
            _context = context;
        }

        [JobDisplayName("Send console log")]

        public async Task Execute()
        {
            _logger.LogInformation("StartKaspiParceJob:" + DateTime.Now);
            var links = _context.Links.ToList();
            foreach (var link in links)
            {
               await ParcerCode(link);
            }
        }

        private async Task ParcerCode(Link link)
        {
            List<SellerInfoDto> sellers = new List<SellerInfoDto>();
            IWebDriver driver = new ChromeDriver();
            try
            {
                driver.Navigate().GoToUrl(link.Url);

                Thread.Sleep(10000);
                var sellersElements = driver.FindElements(By.XPath("//tbody/tr"));
                foreach (var element in sellersElements)
                {
                    var sellerName = element.FindElement(By.CssSelector(".sellers-table__cell a")).Text;
                    var priceString = element.FindElement(By.CssSelector(".sellers-table__price-cell-text")).Text;

                    sellers.Add(new SellerInfoDto
                    {
                        Name = sellerName,
                        Price = ConvertPriceToLong(priceString)
                    });
                }
                
                var specElements = driver.FindElements(By.CssSelector("ul.short-specifications li.short-specifications__text"));
                var specName = new StringBuilder();
                foreach (var element in specElements)
                {
                    specName.Append(element.Text + " ");
                }
                var productName = driver.FindElement(By.CssSelector("h1.item__heading")).Text;

                foreach (var seller in sellers)
                {
                    var supplier = new Supplier()
                    {
                        Name = seller.Name
                    };
                    _context.Suppliers.Add(supplier);

                    var product = new Product()
                    {
                        Name = productName,
                        Description = specName.ToString(),
                        LinkId = link.Id
                    };
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    var productSupplier = new ProductSupplier()
                    {
                        ProductId = product.Id,
                        SupplierId = supplier.Id
                    };
                    _context.ProductSuppliers.Add(productSupplier);

                    var price = new Price()
                    {
                        Value = seller.Price.ToString()
                    };
                    _context.Prices.Add(price);
                    await _context.SaveChangesAsync();

                    var priceProductSupplier = new PriceProductSupplier
                    {
                        PriceId = price.Id, 
                        ProductSupplierId = productSupplier.Id,
                    };
                    _context.PriceProductSuppliers.Add(priceProductSupplier);

                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Seller: {seller.Name}, Price: {seller.Price}");
                }
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
