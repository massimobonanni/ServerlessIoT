using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerlessIoT.Core;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TelemetryEntities.Entities;
using TelemetryEntities.Filters;
using TelemetryEntities.Models;

namespace TelemetryEntities
{
    public class DeviceEntityManagement
    {
        [FunctionName(nameof(SendTelemetryToDevice))]
        public async Task<IActionResult> SendTelemetryToDevice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "deviceTelemetries")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var telemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(requestBody);

            var entityId = new EntityId(nameof(DeviceEntity), telemetry.DeviceId);

            await client.SignalEntityAsync(entityId,
                   nameof(DeviceEntity.TelemetryReceived), telemetry);

            return new OkObjectResult(telemetry);
        }

        [FunctionName(nameof(GetDevices))]
        public static async Task<IActionResult> GetDevices(
          [HttpTrigger(AuthorizationLevel.Function, "get", Route = "devices")] HttpRequest req,
          [DurableClient] IDurableEntityClient client)
        {
            var result = new List<DeviceInfoModel>();

            var filters = GetDevicesFilters.CreateFromHttpRequest(req);

            var queryDefinition = new EntityQuery()
            {
                EntityName = nameof(DeviceEntity),
                PageSize = 100,
                FetchState = true
            };

            do
            {
                var queryResult = await client.ListEntitiesAsync(queryDefinition, default);

                foreach (var item in queryResult.Entities)
                {
                    var device = item.ToDeviceInfoModel();
                    if (filters.IsDeviceValid(device))
                        result.Add(item.ToDeviceInfoModel());
                }

                queryDefinition.ContinuationToken = queryResult.ContinuationToken;
            } while (queryDefinition.ContinuationToken != null && queryDefinition.ContinuationToken != "bnVsbA==");

            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetDevice))]
        public static async Task<IActionResult> GetDevice(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "devices/{deviceId}")] HttpRequest req,
            string deviceId,
            [DurableClient] IDurableEntityClient client)
        {
            var entityId = new EntityId(nameof(DeviceEntity), deviceId);

            var entity = await client.ReadEntityStateAsync<JObject>(entityId);
            if (entity.EntityExists)
            {
                var device = entity.EntityState.ToDeviceDetailModel();
                device.DeviceId = deviceId;
                return new OkObjectResult(device);
            }
            return new NotFoundObjectResult(deviceId);
        }

        [FunctionName(nameof(SetConfiguration))]
        public static async Task<IActionResult> SetConfiguration(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "devices/{deviceId}/configuration")] HttpRequest req,
            string deviceId,
            [DurableClient] IDurableEntityClient client)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var configuration = JsonConvert.DeserializeObject<DeviceEntityConfiguration>(requestBody);

            var entityId = new EntityId(nameof(DeviceEntity), deviceId);

            await client.SignalEntityAsync(entityId,
                   nameof(DeviceEntity.SetConfiguration), configuration);

            return new OkObjectResult(configuration);
        }
    }

}
