using System;
using System.Collections.Generic;
using System.Text;

namespace VAProject.Core.Models.Notifications
{
    public class WeatherPayload : NotificationPayload
    {
        public string City { get; set; }
        public int Temperature { get; set; }
        public string Description { get; set; }
        public bool IsSunny { get; set; }
    }
}
