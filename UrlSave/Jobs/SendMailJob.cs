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

        public SendMailJob(ILogger<SendMailJob> logger, LinkContext context)
        {
            _logger = logger;
            _context = context;
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

        private async Task SendEmail(Notification notification)
        {
            var recipientEmail = notification.Recipient;
            var emailSubject = $"Уведомление об изменении цены: {notification.Title}";
            var emailBody = notification.Body;

            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587; // или порт, который использует ваш почтовый провайдер
                    smtpClient.Credentials = new NetworkCredential("", "*******");
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(""),
                        Subject = emailSubject,
                        Body = emailBody,
                        IsBodyHtml = true, // если ваше письмо в формате HTML
                    };

                    mailMessage.To.Add(recipientEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Письмо отправлено на {recipientEmail}: {emailSubject}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при отправке письма: {ex.Message}");
            }
        }
    }
}