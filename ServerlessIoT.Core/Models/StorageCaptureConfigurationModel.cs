using Newtonsoft.Json;

namespace TelemetryEntities.Models
{
    public class StorageCaptureConfigurationModel
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty("timeWindowInMinutes")]
        public int TimeWindowInMinutes { get; set; } = 5;
    }
}
