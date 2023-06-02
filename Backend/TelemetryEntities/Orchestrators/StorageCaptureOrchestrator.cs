using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ServerlessIoT.Core;
using TelemetryEntities.Activities;
using TelemetryEntities.Models;

namespace TelemetryEntities.Orchestrators
{
    public class StorageCaptureOrchestrator
    {
        [FunctionName(nameof(SaveCapture))]
        public async Task SaveCapture(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var capturedata = context.GetInput<StorageCaptureData>();

            await context.CallActivityAsync(nameof(StorageActivities.SaveCaptureData),
                capturedata);

        }

    }
}