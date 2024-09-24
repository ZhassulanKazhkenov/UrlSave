using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UrlSave.Contexts;
using UrlSave.Entities;
using UrlSave.Jobs;
using Xunit;

namespace UrlSave.Tests
{
    public class ParceKaspiJobTests
    {
        private DbContextOptions<LinkContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<LinkContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
        }

        [Fact]
        public async Task ParseKaspiLinks_ShouldParseLinksAndAddProductsAndPrices()
        {
            var options = GetInMemoryOptions();
            var loggerMock = new Mock<ILogger<ParceKaspiJob>>();
            var productServiceMock = new Mock<ProductService>();

            using (var context = new LinkContext(options))
            {
                var link = new Link("http://example.com", 1, null);
                context.Links.Add(link);
                await context.SaveChangesAsync();

                var job = new ParceKaspiJob(loggerMock.Object, context, productServiceMock.Object);
                await job.ParseKaspiLinks();

                var products = await context.Products.ToListAsync();
                var prices = await context.Prices.ToListAsync();

                Assert.Single(products);
                Assert.Single(prices);
                Assert.Equal(link.ProductId, products.First().Id);
                Assert.Equal(products.First().Id, prices.First().ProductId);
            }
        }
    }
}