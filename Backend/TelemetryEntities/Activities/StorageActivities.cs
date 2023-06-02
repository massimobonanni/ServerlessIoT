using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using System;
using System.Collections.Generic;
using System.Text;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using TelemetryEntities.Models;
using System.Threading.Tasks;
using TelemetryEntities.Services;

namespace TelemetryEntities.Activities
{
    public class StorageActivities
    {
        private readonly IStorageService storageService;
        public StorageActivities(IStorageService storageService)
        {
            this.storageService=storageService;
        }

        [FunctionName(nameof(SaveCaptureData))]
        public async Task SaveCaptureData([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var data = context.GetInput<StorageCaptureData>();

            log.LogInformation($"Capture data for device {data.DeviceId}");

            var saveresult=await this.storageService.SaveCaptureDataAsync(data);

        }
    }
}
