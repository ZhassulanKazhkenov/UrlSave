namespace UrlSave.Interfaces
{
    public interface ISendMailJob
    {
        Task SendPendingNotifications();
        Task<bool> SendNotificationEmailAsync(Notification notification);
    }
}
