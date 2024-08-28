namespace UrlSave.Jobs
{
    public interface ISendMailJob
    {
        Task SendPendingNotifications();
        Task<bool> SendNotificationEmailAsync(Notification notification);
    }
    public class SendMailJob : ISendMailJob
    {
        private readonly ILogger<SendMailJob> _logger;
        private readonly LinkContext _context;
        private readonly IConfiguration _configuration;

        public SendMailJob(ILogger<SendMailJob> logger, LinkContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [JobDisplayName("SendMailJob")]
        public async Task SendPendingNotifications()
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => !n.IsSend)
                    .ToListAsync();
                _logger.LogInformation($"Notifications to Send: {notifications.Count}");

                foreach (var notification in notifications)
                {
                    var isSend = await SendNotificationEmailAsync(notification);
                    if (isSend)
                    {
                        notification.IsSend = true;
                        _context.Notifications.Update(notification);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SendMailJob: {ex.Message}");
            }
        }

        public async Task<bool> SendNotificationEmailAsync(Notification notification)
        {
            try
            {
                var recipientEmail = notification.Recipient;
                _logger.LogInformation($"Try to send: {recipientEmail}");

                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPass"];
                var senderEmail = _configuration["Email:SenderEmail"];
                var host = _configuration["Email:Host"];
                var port = int.Parse(_configuration["Email:Port"]);
                using var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = notification.Title,
                    Body = notification.Body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(recipientEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email is sent to {recipientEmail}: {notification.Title}");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"SMTP failed: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error when send email: {ex.Message}");
                return false;
            }
        }
    }
}
