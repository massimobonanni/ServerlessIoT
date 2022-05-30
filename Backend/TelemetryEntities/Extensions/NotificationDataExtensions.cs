using System;

namespace TelemetryEntities.Models
{
    public static class NotificationDataExtensions
    {
        public static string CreateMessage(this NotificationData notificationData)
        {
            if (notificationData == null)
                throw new NullReferenceException(nameof(notificationData));

            return notificationData.Type switch
            {
                NotificationType.LowTemperature => $"Temperature {notificationData.CurrentValue:0.00} less then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.",
                NotificationType.HighTemperature => $"Temperature {notificationData.CurrentValue:0.00} greater then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.",
                NotificationType.LowHumidity => $"Humidity {notificationData.CurrentValue:0.00} less then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.",
                NotificationType.HighHumidity => $"Humidity {notificationData.CurrentValue:0.00} greater then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.",
                _ => "",
            };
        }
    }
}
