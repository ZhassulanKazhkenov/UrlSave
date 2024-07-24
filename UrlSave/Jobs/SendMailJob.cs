using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
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
                var unsentNotifications = _context.Notifications
                    .Where(n => !n.IsSend)
                    .ToList();

                foreach (var notification in unsentNotifications)
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
            // Предполагаем, что у нас есть метод для отправки электронных писем
            // Получаем адрес электронной почты получателя (из User или напрямую из Notification)
            var recipientEmail = notification.Recipient;

            // Формируем содержание письма (например, тему и текст)
            var emailSubject = $"Уведомление об изменении цены: {notification.Title}";
            var emailBody = notification.Body;

            // Отправляем письмо
            // Пример: SendEmail(recipientEmail, emailSubject, emailBody);
            // Реализуйте свою логику отправки писем здесь
            // ...

            _logger.LogInformation($"Письмо отправлено на {recipientEmail}: {emailSubject}");
        }
    }
}