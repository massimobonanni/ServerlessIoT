using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryEntities.Models
{
    public class NotificationData
    {
        public DateTimeOffset Timestamp { get; set; }
        public double? CurrentValue { get; set; }
        public double? ThresholdValue { get; set; }
        public NotificationType Type { get; set; }
        public string NotificationNumber { get; set; }
        public string DeviceName { get; set; }
    }

    public enum NotificationType
    {
        Unknown,
        HighTemperature,
        LowTemperature,
        HighHumidity,
        LowHumidity
    }
}
