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
    public class DeviceEntityManagementRestProvider : RestClientBase, ITelemetryManager
    {

        public DeviceEntityManagementRestProvider(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {

        }

        protected Uri CreateAPIUri(string apiEndpoint, bool overrideUri = false)
        {
            Uri uri;
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                uri = base.CreateAPIUri($"/api/devices");
            }
            else
            {
                if (apiEndpoint.StartsWith("/"))
                    apiEndpoint = apiEndpoint.Remove(0, 1);
                if (overrideUri)
                    uri = base.CreateAPIUri($"/{apiEndpoint}");
                else
                    uri = base.CreateAPIUri($"/api/devices/{apiEndpoint}");
            }

            return uri;
        }

        public async Task<bool> SendTelemetryToDeviceAsync(DeviceTelemetry telemetry, CancellationToken cancellationToken)
        {
            if (telemetry == null)
                throw new ArgumentNullException(nameof(telemetry));

            var uri = this.CreateAPIUri("api/deviceTelemetries", true);

            string telemetryJson = JsonConvert.SerializeObject(telemetry, Formatting.None);

            var postContent = new StringContent(telemetryJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DeviceInfoModel>> GetDevicesAsync(CancellationToken token)
        {
            var uri = this.CreateAPIUri("",false);
            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var profiles = JsonConvert.DeserializeObject<List<DeviceInfoModel>>(content);

                return profiles;
            }
            return null;
        }

        public async Task<DeviceInfoModel> GetDeviceAsync(string deviceId, CancellationToken token)
        {
            var uri = this.CreateAPIUri($"{deviceId}",false);

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var profile = JsonConvert.DeserializeObject<DeviceInfoModel>(content);

                return profile;
            }
            return null;

        }
    }
}
