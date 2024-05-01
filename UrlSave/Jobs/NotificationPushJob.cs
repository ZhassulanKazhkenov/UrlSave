using Hangfire;
using Microsoft.EntityFrameworkCore;
using UrlSave.Contexts;

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

            //var links = _context.Links
            //    .Where(x => x.ProductId != null)
            //    .Include(x => x.Product)
            //    .ThenInclude(x => x.PriceProductSupplier)
            //    .ThenInclude(x => x.Price)
            //    .ToList();

        }
    }
}