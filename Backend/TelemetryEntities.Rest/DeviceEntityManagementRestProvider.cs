﻿using ServerlessIoT.Core;
using ServerlessIoT.Core.Interfaces;
using StatefulPatternFunctions.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryEntities.Rest
{
    public class DeviceEntityManagementRestProvider : RestClientBase, ITelemetryManager
    {

        public DeviceEntityManagementRestProvider(HttpClient httpClient, string baseUrl, string apiKey):
            base(httpClient,baseUrl,apiKey)
        {

        }

        protected override Uri CreateAPIUri(string apiEndpoint)
        {
            Uri uri;
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                uri = base.CreateAPIUri($"/api/deviceTelemetries");
            }
            else
            {
                if (apiEndpoint.StartsWith("/"))
                    apiEndpoint = apiEndpoint.Remove(0, 1);
                uri = base.CreateAPIUri($"/api/deviceTelemetries/{apiEndpoint}");
            }

            return uri;
        }

        public async Task<bool> SendTelemetryToDeviceAsync(DeviceTelemetry telemetry, CancellationToken cancellationToken)
        {
            if (telemetry == null)
                throw new ArgumentNullException(nameof(telemetry));

            var uri = this.CreateAPIUri("");

            string telemetryJson = JsonSerializer.Serialize(telemetry,
              new JsonSerializerOptions
              {
                  PropertyNameCaseInsensitive = true,
              });

            var postContent = new StringContent(telemetryJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }
    }
}