namespace UrlSave.Tests;
public class ParceKaspiJobTests
{
    private DbContextOptions<LinkContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<LinkContext>()
            .UseInMemoryDatabase("InMemoryDb")
            .Options;
    }

    [Fact]
    public async Task ShouldParseLinksAndAddProductsAndPrices()
    {
        var options = GetInMemoryOptions();
        var loggerMock = new Mock<ILogger<ParceKaspiJob>>();
        var productServiceMock = new Mock<ProductService>(MockBehavior.Loose, new LinkContext(options));

        using (var context = new LinkContext(options))
        {
            var link = new Link("https://kaspi.kz/shop/p/apple-18w-usb-c-power-adapter-usb-type-c-belyi-102743952/?c=710000000", 1, null);
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