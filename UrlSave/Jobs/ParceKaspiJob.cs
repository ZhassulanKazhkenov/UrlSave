using Hangfire;
using Hangfire.Dashboard;
using System.Net.Http;
using System;
using UrlSave.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UrlSave.Contexts;
using UrlSave.Entities;

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

        public void Execute()
        {
            _logger.LogInformation("StartKaspiParceJob:" + DateTime.Now);
            var links = _context.Links.ToList();
            foreach (var link in links)
            {
                ParcerCode(link);
            }
        }

        private void ParcerCode(Link link)
        {
            List<SellerInfoDto> sellers = new List<SellerInfoDto>();
            IWebDriver driver = new ChromeDriver();
            try
            {
                driver.Navigate().GoToUrl(link.Url);

                Thread.Sleep(5000);

                var priceElement = driver.FindElement(By.ClassName("item__price-once"));
                string price = priceElement.Text;

                Console.WriteLine($"Price is: {ConvertPriceToLong(price)}");

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

                // Выводим информацию о каждом продавце
                foreach (var seller in sellers)
                {
                    var s = new Supplier()
                    {
                        Name = seller.Name
                    };
                    _context.Suppliers.Add(s);
                    var pr = new Product()
                    {
                        Name = "",
                        Description = "",
                        LinkId = link.Id
                    };
                    _context.Products.Add(pr);
                    var pS = new ProductSupplier()
                    {
                        ProductId = pr.Id,
                        SupplierId = s.Id
                    };
                    _context.ProductSuppliers.Add(pS);
                    var p = new Price()
                    {
                        Value = seller.Price.ToString()
                    };
                    _context.Prices.Add(p);
                    var PPS = new PriceProductSupplier
                    {
                        PriceId = p.Id, 
                        ProductSupplierId = pS.Id,
                    };
                    _context.PriceProductSuppliers.Add(PPS);
                    _context.SaveChanges();
                    Console.WriteLine($"Seller: {seller.Name}, Price: {seller.Price}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                driver.Quit(); // Закрыть браузер и освободить ресурсы
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
