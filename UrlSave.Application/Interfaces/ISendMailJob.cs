using UrlSave.Domain.Entities;

namespace UrlSave.Application.Interfaces;

public interface ISendMailJob
{
    Task SendPendingNotifications();
    Task<bool> SendNotificationEmailAsync(Notification notification);
}
