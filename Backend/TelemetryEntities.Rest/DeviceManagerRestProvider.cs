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
    public class DeviceManagerRestProvider : RestClientBase, IDeviceManager
    {

        public DeviceManagerRestProvider(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {

        }

        protected override string BaseEndpoint => "api/devices";

        public async Task<IEnumerable<DeviceInfoModel>> GetDevicesAsync(CancellationToken token)
        {
            var uri = this.CreateAPIUri();
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
            var uri = this.CreateAPIUri($"{deviceId}");

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
