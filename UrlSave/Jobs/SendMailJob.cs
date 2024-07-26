using System.Net;
using System.Net.Mail;
using Hangfire;
using UrlSave.Contexts;
using UrlSave.Entities;

namespace UrlSave.Jobs
{
    public class SendMailJob
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
        public async Task Execute()
        {
            _logger.LogInformation("Start SendMailJob: " + DateTime.Now);

            try
            {
                // Получаем уведомления с IsSend = false
                var notifications = _context.Notifications
                    .Where(n => !n.IsSend)
                    .ToList();
                _logger.LogInformation($"Notifications to Send: {notifications.Count}");

                foreach (var notification in notifications)
                {
                    // Предполагаем, что у нас есть метод для отправки электронных писем
                    await SendEmail(notification);

                    // Устанавливаем статус IsSend в true
                    notification.IsSend = true;
                    _context.Notifications.Update(notification);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в SendMailJob: {ex.Message}");
            }
        }

        private async Task<bool> SendEmail(Notification notification)
        {
            var recipientEmail = notification.Recipient;
            var emailSubject = $"Уведомление об изменении цены: {notification.Title}";
            var emailBody = notification.Body;

            try
            {
                _logger.LogInformation($"Try to send: {recipientEmail}");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPass"];
                var senderEmail = _configuration["Email:SenderEmail"];
                using (var smtpClient = new SmtpClient("smtp.mail.ru"))
                {
                    smtpClient.Port = 465;
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail),
                        Subject = emailSubject,
                        Body = emailBody,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(recipientEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Письмо отправлено на {recipientEmail}: {emailSubject}");
                }
                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP ошибка при отправке письма: {smtpEx.Message}", smtpEx);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при отправке письма: {ex.Message}", ex);
                return false;
            }
        }
    }
}