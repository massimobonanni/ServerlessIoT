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

        protected override string DefaultApiEndpoint => "api/devices";

        public async Task<IEnumerable<DeviceInfoModel>> GetDevicesAsync(string filterName, string filterId, CancellationToken token)
        {
            string query = string.Empty;
            if (!string.IsNullOrEmpty(filterName))
            {
                query += $"name={filterName}";
            }
            if (!string.IsNullOrEmpty(filterId))
            {
                if (query != null)
                    query += $"&";
                query += $"id={filterId}";
            }

            Uri uri;
            if (!string.IsNullOrEmpty(query))
                uri = this.CreateAPIUri($"{query}");
            else
                uri = this.CreateAPIUri();

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var profiles = JsonConvert.DeserializeObject<List<DeviceInfoModel>>(content);

                return profiles;
            }
            return null;
        }

        public async Task<DeviceDetailModel> GetDeviceAsync(string deviceId, CancellationToken token)
        {
            var uri = this.CreateAPIUri(null,$"api/devices/{deviceId}");

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var profile = JsonConvert.DeserializeObject<DeviceDetailModel>(content);

                return profile;
            }
            return null;

        }
    }
}
