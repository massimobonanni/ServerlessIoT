using Azure.Messaging.EventHubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
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
using TelemetryEntities.Services;

using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

namespace TelemetryEntities
{
    public class DeviceEntityManagement
    {
        private readonly IEntityFactory entityfactory;
        private readonly IIotHubService iotHubService;

        public DeviceEntityManagement(IEntityFactory entityFactory, IIotHubService iotHubService)
        {
            this.entityfactory = entityFactory;
            this.iotHubService = iotHubService;
        }

        [FunctionName(nameof(IoTHubDispatcher))]
        public async Task IoTHubDispatcher(
            [IoTHubTrigger("%IotHubName%", Connection = "EventHubCompatibleConnectionString")] EventData[] eventHubMessages,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            foreach (var message in eventHubMessages)
            {
                var telemetry = message.ExtractDeviceTelemetry(logger);
                logger.LogTelemetryInfo(telemetry);
                var entityId = await this.entityfactory.GetEntityIdAsync(telemetry.DeviceId, default);
                await client.SignalEntityAsync<IDeviceEntity>(entityId, d => d.TelemetryReceived(telemetry));
            }
        }

        [OpenApiOperation("sendTelemetryToDevice",
            new[] { "Telemetries" },
            Summary = "Send a telemetry to a serverless device",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody("application/json",
            typeof(DeviceTelemetry),
            Description = "The telemetry to send to the serverless device",
            Required = true)]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK,
            "application/json",
            typeof(DeviceTelemetry),
            Summary = "The telemetry passed to the request")]
        [OpenApiSecurity("apikeyquery_auth",
            SecuritySchemeType.ApiKey,
            In = OpenApiSecurityLocationType.Query,
            Name = "code")]

        [FunctionName(nameof(SendTelemetryToDevice))]
        public async Task<IActionResult> SendTelemetryToDevice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "deviceTelemetries")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var telemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(requestBody);

            var entityId = await this.entityfactory.GetEntityIdAsync(telemetry.DeviceId, default);

            await client.SignalEntityAsync<IDeviceEntity>(entityId, d => d.TelemetryReceived(telemetry));

            return new OkObjectResult(telemetry);
        }

        [OpenApiOperation("getDevices",
           new[] { "Devices" },
           Summary = "Search the devices based on device name or id",
            Description = "Return the list of devices those match the search criteria.",
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("name",
            Summary = "Name of the device",
            Description = "Returns the devices with name taht contains the value.",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = false,
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("id",
            Summary = "Identifier of the device",
            Description = "Returns the devices with the id that contains the value.",
            In = Microsoft.OpenApi.Models.ParameterLocation.Query,
            Required = false,
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK,
            "application/json",
            typeof(List<DeviceInfoModel>),
            Summary = "The list of the devices matching the filters")]
        [OpenApiSecurity("apikeyquery_auth",
            SecuritySchemeType.ApiKey,
            In = OpenApiSecurityLocationType.Query,
            Name = "code")]

        [FunctionName(nameof(GetDevices))]
        public async Task<IActionResult> GetDevices(
          [HttpTrigger(AuthorizationLevel.Function, "get", Route = "devices")] HttpRequest req,
          [DurableClient] IDurableEntityClient client)
        {
            var result = new List<DeviceInfoModel>();

            var filters = GetDevicesFilters.CreateFromHttpRequest(req);

            var queryDefinition = new EntityQuery()
            {
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
                        result.Add(device);
                }

                queryDefinition.ContinuationToken = queryResult.ContinuationToken;
            } while (queryDefinition.ContinuationToken != null && queryDefinition.ContinuationToken != "bnVsbA==");

            return new OkObjectResult(result);
        }

        [OpenApiOperation("getDevice",
           new[] { "Devices" },
           Summary = "Get a specific device by id",
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("deviceId",
            Summary = "Identifier of the device",
            In = Microsoft.OpenApi.Models.ParameterLocation.Path,
            Required = true,
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK,
            "application/json",
            typeof(DeviceDetailModel),
            Summary = "The info about the device")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.NotFound,
            "application/json",
            typeof(string),
            Summary = "The device id if the device doesn't exist")]
        [OpenApiSecurity("apikeyquery_auth",
            SecuritySchemeType.ApiKey,
            In = OpenApiSecurityLocationType.Query,
            Name = "code")]

        [FunctionName(nameof(GetDevice))]
        public async Task<IActionResult> GetDevice(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "devices/{deviceId}")] HttpRequest req,
            string deviceId,
            [DurableClient] IDurableEntityClient client)
        {

            var entityId = await this.entityfactory.GetEntityIdAsync(deviceId, default);

            var entity = await client.ReadEntityStateAsync<JObject>(entityId);
            if (entity.EntityExists)
            {
                var device = entity.EntityState.ToDeviceDetailModel();
                device.DeviceId = deviceId;
                return new OkObjectResult(device);
            }
            return new NotFoundObjectResult(deviceId);
        }

        [OpenApiOperation("setConfiguration",
           new[] { "Devices" },
           Summary = "Set the configuration data for a specific device",
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("deviceId",
            Summary = "Identifier of the device",
            In = Microsoft.OpenApi.Models.ParameterLocation.Path,
            Required = true,
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody("application/json",
            typeof(DeviceEntityConfiguration),
            Description = "The device configuration data to set",
            Required = true)]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK,
            "application/json",
            typeof(DeviceEntityConfiguration),
            Summary = "The device configuration data setted")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.NotFound,
            "application/json",
            typeof(string),
            Summary = "The device id if the device doesn't exist")]
        [OpenApiSecurity("apikeyquery_auth",
            SecuritySchemeType.ApiKey,
            In = OpenApiSecurityLocationType.Query,
            Name = "code")]

        [FunctionName(nameof(SetConfiguration))]
        public async Task<IActionResult> SetConfiguration(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "devices/{deviceId}/configuration")] HttpRequest req,
            string deviceId,
            [DurableClient] IDurableEntityClient client)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var configuration = JsonConvert.DeserializeObject<DeviceEntityConfiguration>(requestBody);

            var entityId = await this.entityfactory.GetEntityIdAsync(deviceId, default);

            var entity = await client.ReadEntityStateAsync<JObject>(entityId);
            if (entity.EntityExists)
            {
                await client.SignalEntityAsync<IDeviceEntity>(entityId, d => d.SetConfiguration(configuration));
                return new OkObjectResult(configuration);
            }
            return new NotFoundObjectResult(deviceId);
        }


        [OpenApiOperation("getConfiguration",
          new[] { "Devices" },
          Summary = "Get the configuration data for a specific device",
          Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("deviceId",
           Summary = "Identifier of the device",
           In = Microsoft.OpenApi.Models.ParameterLocation.Path,
           Required = true,
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK,
            "application/json",
            typeof(DeviceEntityConfiguration),
            Summary = "The device configuration data retrieved")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.NotFound,
            "application/json",
            typeof(string),
            Summary = "The device id if the device doesn't exist")]
        [OpenApiSecurity("apikeyquery_auth",
            SecuritySchemeType.ApiKey,
            In = OpenApiSecurityLocationType.Query,
            Name = "code")]

        [FunctionName(nameof(GetConfiguration))]
        public async Task<IActionResult> GetConfiguration(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "devices/{deviceId}/configuration")] HttpRequest req,
            string deviceId,
            [DurableClient] IDurableEntityClient client)
        {
            var entityId = await this.entityfactory.GetEntityIdAsync(deviceId, default);

            var entity = await client.ReadEntityStateAsync<JObject>(entityId);
            if (entity.EntityExists)
            {
                var configuration = entity.EntityState.ExtractDeviceConfiguration();
                return new OkObjectResult(configuration);
            }
            return new NotFoundObjectResult(deviceId);
        }

        [OpenApiOperation("callMethodDevice",
           new[] { "Devices" },
           Summary = "Call a method of a device",
           Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("deviceId",
            Summary = "Identifier of the device",
            In = Microsoft.OpenApi.Models.ParameterLocation.Path,
            Required = true,
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody("application/json",
            typeof(DeviceMethodModel),
            Description = "The method to call",
            Required = true)]
        [OpenApiResponseWithoutBody(System.Net.HttpStatusCode.OK,
            Summary = "The method is called correctly")]
        [OpenApiResponseWithoutBody(System.Net.HttpStatusCode.NotFound,
            Summary = "The device doesn't exist")]
        [OpenApiResponseWithoutBody(System.Net.HttpStatusCode.BadRequest,
            Summary = "The method payload is not valid")]
        [OpenApiSecurity("apikeyquery_auth",
            SecuritySchemeType.ApiKey,
            In = OpenApiSecurityLocationType.Query,
            Name = "code")]

        [FunctionName(nameof(CallMethodDevice))]
        public async Task<IActionResult> CallMethodDevice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices/{deviceId}/method")] HttpRequest req,
            string deviceId,
            [DurableClient] IDurableEntityClient client)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var method = JsonConvert.DeserializeObject<DeviceMethodModel>(requestBody);

            if (method == null || string.IsNullOrEmpty(method.Method))
            {
                return new BadRequestResult();
            }

            var entityId = await this.entityfactory.GetEntityIdAsync(deviceId, default);

            var entity = await client.ReadEntityStateAsync<JObject>(entityId);
            if (entity.EntityExists)
            {
                await this.iotHubService.InvokeDeviceMethodAsync(deviceId,
                    method.Method, method.Payload);

                return new OkResult();
            }
            return new NotFoundResult();
        }
    }

}
