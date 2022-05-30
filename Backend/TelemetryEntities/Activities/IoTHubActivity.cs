using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryEntities.Models;
using TelemetryEntities.Services;

namespace TelemetryEntities.Activities
{
    public class IoTHubActivity
    {
        private readonly IIotHubService iotHubService;
        public IoTHubActivity(IIotHubService iotHubService)
        {
            this.iotHubService = iotHubService;
        }

        [FunctionName(nameof(SendMessageToDevice))]
        public async Task SendMessageToDevice([ActivityTrigger] NotificationData notificationData, ILogger log)
        {
            var message = notificationData.CreateMessage();
            await this.iotHubService.SendMessageToDeviceAsync(notificationData.DeviceId, message);
        }
    }
}
