using Hangfire;
using Microsoft.EntityFrameworkCore;
using UrlSave.Contexts;
using UrlSave.Entities;

namespace UrlSave.Jobs
{
    public class NotificationPushJob
    {
        private readonly ILogger<NotificationPushJob> _logger;
        private readonly LinkContext _context;

        public NotificationPushJob(ILogger<NotificationPushJob> logger, LinkContext context)
        {
            _logger = logger;
            _context = context;
        }

        [JobDisplayName("NotificationPushJob")]
        public async Task Execute()
        {
            _logger.LogInformation("StartNotificationPushJob:" + DateTime.Now);

            var links = _context.Links
                .Where(x => x.ProductId != null)
                .Include(x => x.User)
                .Include(x => x.Product)
                .ThenInclude(x => x.Prices)
                .ToList();

            foreach ( var link in links)
            {
                var prices = link
                    .Product
                    .Prices
                    .OrderBy(x => x.CreatedDate)
                    .ToList();

                var shouldNotify = prices.FirstOrDefault().Value != prices.LastOrDefault().Value;
                if (shouldNotify)
                {
                    var notification = new Notification
                    {
                        Title = $"Price is changed for {link.Product.Name}",
                        Body = $"Your notification about price changing. First price is: {prices.FirstOrDefault().Value}, New price is: {prices.LastOrDefault().Value}",
                        Recipient = link.User.Email,
                        IsSend = false,
                        Link = link,
                    };
                    await _context.Notifications.AddAsync(notification);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
