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

            await context.CallActivityAsync(nameof(IoTHubActivity.SendMessageToDevice), 
                notificationdata);

            if (!string.IsNullOrEmpty(notificationdata.NotificationNumber))
                await context.CallActivityAsync(nameof(TwilioActivities.SendMessageToTwilio), 
                    notificationdata);

        }

    }
}