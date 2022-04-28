using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TelemetryEntities.Activities;
using TelemetryEntities.Models;

namespace TelemetryEntities.Orchestrators
{
    public class NotificationOrchestrator
    {
        [FunctionName(nameof(SendNotification))]
        public async Task SendNotification(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var notificationdata = context.GetInput<NotificationData>();

            var smsData = new TwilioActivities.SmsData()
            {
                Destination = notificationdata.NotificationNumber,
                Message = CreateMessageFromNotificationdata(notificationdata)
            };

            try
            {
                await context.CallActivityAsync(nameof(TwilioActivities.SendMessageToTwilio), smsData);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Error during TwilioActivity invocation", smsData);
            }
        }

        private string CreateMessageFromNotificationdata(NotificationData notificationData)
        {
            switch (notificationData.Type)
            {
                case NotificationType.LowTemperature:
                    return $"Temperature {notificationData.CurrentValue:0.00} less then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.";
                case NotificationType.HighTemperature:
                    return $"Temperature {notificationData.CurrentValue:0.00} greater then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.";
                case NotificationType.LowHumidity:
                    return $"Humidity {notificationData.CurrentValue:0.00} less then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.";
                case NotificationType.HighHumidity:
                    return $"Humidity {notificationData.CurrentValue:0.00} greater then {notificationData.ThresholdValue:0.00} from {notificationData.DeviceName} at {notificationData.Timestamp}.";
                case NotificationType.Unknown:
                default:
                    return "";
            }
        }
    }
}