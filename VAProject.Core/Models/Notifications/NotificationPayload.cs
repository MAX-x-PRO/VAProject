using System;
using System.Collections.Generic;
using System.Text;

namespace VAProject.Core.Models.Notifications
{
    public abstract class NotificationPayload
    {
        public int DurationMs { get; set; } = 3000;
    }
}
