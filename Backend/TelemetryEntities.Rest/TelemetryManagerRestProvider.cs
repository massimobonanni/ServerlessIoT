using Newtonsoft.Json;
using ServerlessIoT.Core;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using StatefulPatternFunctions.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryEntities.Rest
{
    public class TelemetryManagerRestProvider : RestClientBase, ITelemetryManager
    {
        public TelemetryManagerRestProvider(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {

        }

        protected override string BaseEndpoint => "api/deviceTelemetries";

        public async Task<bool> SendTelemetryToDeviceAsync(DeviceTelemetry telemetry, CancellationToken cancellationToken)
        {
            if (telemetry == null)
                throw new ArgumentNullException(nameof(telemetry));

            var uri = this.CreateAPIUri();

            string telemetryJson = JsonConvert.SerializeObject(telemetry, Formatting.None);

            var postContent = new StringContent(telemetryJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        
    }
}
