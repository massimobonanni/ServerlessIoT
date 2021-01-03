using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TelemetrySimulator
{
    internal class DevicesConfiguration
    {
        [JsonPropertyName("devices")]
        public IEnumerable<DeviceConfiguration> Devices { get; set; }
    }

    public class DeviceConfiguration
    {
        [JsonPropertyName("connectionString")]
        public string ConnectionString { get; set; }

        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("pollingIntervalInSec")]
        public int PollingIntervalInSec { get; set; }

    }
}
