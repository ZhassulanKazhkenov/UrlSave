using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailSender
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _senderEmail = senderEmail;
            _senderPassword = senderPassword;
        }

        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(_senderEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    message.To.Add(recipientEmail);

                    await client.SendMailAsync(message);
                    Console.WriteLine($"Email sent successfully to {recipientEmail}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // Настройте параметры сервера SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Используйте 465 для SSL или 587 для TLS
            string senderEmail = "ваш_адрес@gmail.com";
            string senderPassword = "ваш_пароль";

            var emailService = new EmailService(smtpServer, smtpPort, senderEmail, senderPassword);

            // Пример использования:
            string recipientEmail = "получатель@example.com";
            string emailSubject = "Уведомление: Изменение цены";
            string emailBody = "Цена продукта изменилась. Проверьте детали.";

            await emailService.SendEmailAsync(recipientEmail, emailSubject, emailBody);
        }
    }
}