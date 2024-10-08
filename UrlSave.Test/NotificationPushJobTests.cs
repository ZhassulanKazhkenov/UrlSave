namespace UrlSave.Tests;

public class NotificationPushJobTests
{
    private DbContextOptions<LinkContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<LinkContext>()
            .UseInMemoryDatabase("InMemoryDb")
            .Options;
    }

    [Fact]
    public async Task ShouldCreateNotification_WhenPriceChanged()
    {
        var options = GetInMemoryOptions();
        var loggerMock = new Mock<ILogger<NotificationPushJob>>();

        using (var context = new LinkContext(options))
        {
            var user = new User("Lilzhas@mail.ru");
            context.Users.Add(user);

            var product = new Product("Test Product", "Description");
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var link = new Link("http://example.com", user.Id, product.Id);
            context.Links.Add(link);


            var price1 = new Price { Value = 100, ProductId = product.Id, CreatedDate = DateTime.Now.AddDays(-1) };
            var price2 = new Price { Value = 200, ProductId = product.Id, CreatedDate = DateTime.Now };
            context.Prices.AddRange(price1, price2);
            await context.SaveChangesAsync();

            var job = new NotificationPushJob(loggerMock.Object, context);
            await job.NotifyPriceChanges();

            var notifications = await context.Notifications.ToListAsync();
            Assert.Single(notifications);
            Assert.Equal("Price is changed for Test Product", notifications.First().Title);
        }
    }

    [Fact]
    public async Task ShouldNotCreateNotification_WhenPriceChanged()
    {
        var options = GetInMemoryOptions();
        var loggerMock = new Mock<ILogger<NotificationPushJob>>();

        using (var context = new LinkContext(options))
        {
            var user = new User("Lilzhas@mail.ru");
            context.Users.Add(user);

            var product = new Product("Test Product", "Description");
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var link = new Link("http://example.com", user.Id, product.Id);
            context.Links.Add(link);


            var price1 = new Price { Value = 100, ProductId = product.Id, CreatedDate = DateTime.Now.AddDays(-1) };
            context.Prices.Add(price1);
            await context.SaveChangesAsync();

            var job = new NotificationPushJob(loggerMock.Object, context);
            await job.NotifyPriceChanges();

            var notifications = await context.Notifications.ToListAsync();
            Assert.Empty(notifications);
        }
    }
}