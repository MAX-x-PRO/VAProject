using VAProject.Core.Models.Notifications;

namespace VAProject.Core.Interfaces
{
    public interface INotificationService
    {
        void ShowWidget(NotificationPayload payload);
    }
}
