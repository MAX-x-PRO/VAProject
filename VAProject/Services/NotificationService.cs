using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;

namespace VAProject.UI.Services
{
    internal class NotificationService : INotificationService
    {
        private NotificationWindow _notificationWindow;

        public NotificationService(NotificationWindow notificationWindow)
        {
            _notificationWindow = notificationWindow;
        }

        public void ShowWidget(NotificationPayload payload)
        {
            _notificationWindow.ShowWidget(payload);
        }
    }
}
