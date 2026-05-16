using VAProject.Core.Logger;
using VAProject.Core.Models.Notifications;

namespace VAProject.Core.CommandsLogic
{
    public class CommandResult
    {
        public bool Success { get; set; }
        public string LogMessage { get; set; }
        public string TTSResponse { get; set; }

        private NotificationPayload _notificationPayload;
        public NotificationPayload NotificationPayload
        {
            get
            {
                return _notificationPayload;
            }
            set
            {
                if (value != null)
                {
                    _notificationPayload = value;
                }
                else
                { 
                    LogManager.Log("NotificationPayload cannot be null.", LogLevel.Warning);
                }
            }
        }
    }
}
