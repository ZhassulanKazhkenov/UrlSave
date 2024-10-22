namespace UrlSave.Jobs.Jobs
{
    public class NotificationPushJob : INotificationPushJob
    {
        private readonly ILogger<NotificationPushJob> _logger;
        private readonly LinkContext _context;

        public NotificationPushJob(ILogger<NotificationPushJob> logger, LinkContext context)
        {
            _logger = logger;
            _context = context;
        }

        [JobDisplayName("NotificationPushJob")]
        public async Task NotifyPriceChanges()
        {
            try
            {
                var links = await _context.Links
                .AsNoTracking()
                .Where(x => x.ProductId != null)
                .Include(x => x.User)
                .Include(x => x.Product)
                .ThenInclude(x => x.Prices)
                .ToListAsync();

                foreach (var link in links)
                {
                    var prices = link
                        .Product.Prices
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();

                    bool hasInsufficientPrices = prices.Count < 2;
                    if (hasInsufficientPrices)
                    {
                        continue;
                    }
                    var lastPrice = prices.First();
                    bool shouldNotify = lastPrice.Value != prices[1]?.Value;
                    var existingNotification = await _context.Notifications
                        .AsNoTracking()
                        .AnyAsync(x => x.PriceId == lastPrice.Id);
                    if (shouldNotify && !existingNotification)
                    {
                        var notification = new Notification
                        {
                            Title = $"Price is changed for {link.Product.Name}",
                            Body = $"Your notification about price changing.<br> Previous price is: {prices[1].Value}, New price is: {lastPrice.Value}<br> Link: <a href='{link.Url}'>{link.Product.Name}</a><br>Raw url: {link.Url}",
                            Recipient = link.User.Email,
                            IsSend = false,
                            LinkId = link.Id,
                            PriceId = lastPrice.Id
                        };
                        await _context.Notifications.AddAsync(notification);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
