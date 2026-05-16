using System;
using System.Collections.Generic;
using System.Text;
using Media = System.Windows.Media;

namespace VAProject.Core.Models.Notifications
{
    public class TextPayload : NotificationPayload
    {
        public string Text { get; set; }
        public Media.Color AccentColor { get; set; } = Media.Colors.White;
    }
}
