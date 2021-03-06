using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TelemetryEntities.Externals
{
    public class Orchestrators
    {
        public class NotificationData
        {
            public string NotificationNumber { get; set; }

            public string DeviceName { get; set; }
        }

        [FunctionName(nameof(SendNotification))]
        public async Task SendNotification(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var notificationdata = context.GetInput<NotificationData>();

            var smsData = new TwilioActivities.SmsData()
            {
                Destination = notificationdata.NotificationNumber,
                Message = $"Alert from device {notificationdata.DeviceName}"
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


    }
}