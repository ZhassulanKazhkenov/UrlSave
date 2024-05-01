namespace UrlSave.Jobs
{
    public class NotificationPushJob
    {
        private readonly ILogger<NotificationPushJob> _logger;

        public NotificationPushJob(ILogger<NotificationPushJob> logger)
        {
            _logger = logger;
        }
    }
}
