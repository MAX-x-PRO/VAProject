using VAProject.Logger;

namespace VAProject.UI
{
    internal static class NotificationManager
    {
        private static NotificationWindow? _notificationWindow;

        public static void Initialize(NotificationWindow notificationWindow)
        {
            if (_notificationWindow != null)
            {
                throw new InvalidOperationException("NotificationManager has already been initialized.");
            }
            _notificationWindow = notificationWindow;
        }

        public static void ShowNotification(string message, System.Windows.Media.Color indicatorColor, int autoHideMilliseconds = 3000)
        {
            if (_notificationWindow != null)
            {
                _notificationWindow.ShowNotification(message, indicatorColor, autoHideMilliseconds);
            }
            else
            {
                LogManager.Log("NotificationManager not initialized with a NotificationWindow instance.", LogLevel.Warning);
            }
        }
    }
}