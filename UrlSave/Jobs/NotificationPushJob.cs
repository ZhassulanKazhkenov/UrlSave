using Hangfire;
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
        }
    }
}
