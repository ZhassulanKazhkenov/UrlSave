using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using UrlSave.Contexts;
using UrlSave.Entities;
using UrlSave.Jobs;
using Xunit;

namespace UrlSave.Tests
{
    public class SendMailJobTests
    {
        private DbContextOptions<LinkContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<LinkContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
        }

        [Fact]
        public async Task SendPendingNotifications_ShouldSendEmailsAndUpdateStatus()
        {
            var options = GetInMemoryOptions();
            var loggerMock = new Mock<ILogger<SendMailJob>>();
            var configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            // Mock configuration values
            configurationMock.SetupGet(c => c.GetSection("Email:SmtpUser").Value).Returns("smtpUser");
            configurationMock.SetupGet(c => c.GetSection("Email:SmtpPass").Value).Returns("smtpPass");
            configurationMock.SetupGet(c => c["Email:SenderEmail"]).Returns("sender@example.com");
            configurationMock.SetupGet(c => c["Email:Host"]).Returns("smtp.example.com");
            configurationMock.SetupGet(c => c["Email:Port"]).Returns("587");

            using (var context = new LinkContext(options))
            {
                var notification = new Notification
                {
                    Title = "Test Notification",
                    Body = "This is a test notification.",
                    Recipient = "recipient@example.com",
                    IsSend = false
                };
                context.Notifications.Add(notification);
                await context.SaveChangesAsync();

                var job = new SendMailJob(loggerMock.Object, context, configurationMock.Object);
                await job.SendPendingNotifications();

                var updatedNotification = await context.Notifications.FirstAsync();
                Assert.True(updatedNotification.IsSend);
            }
        }
    }
}