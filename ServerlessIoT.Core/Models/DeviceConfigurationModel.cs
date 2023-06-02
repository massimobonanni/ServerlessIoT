using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEntities.Models
{
    public class DeviceConfigurationModel
    {
        [JsonProperty("historyRetention")]
        public TimeSpan HistoryRetention { get; set; } = TimeSpan.FromMinutes(10);

        [JsonProperty("temperatureHighThreshold")]
        public double? TemperatureHighThreshold { get; set; }

        [JsonProperty("temperatureLowThreshold")]
        public double? TemperatureLowThreshold { get; set; }

        [JsonProperty("notificationNumber")]
        public string NotificationNumber { get; set; }

        [JsonProperty("temperatureDecimalPrecision")]
        public int TemperatureDecimalPrecision { get; set; } = 2;

        [JsonProperty("humidityDecimalPrecision")]
        public int HumidityDecimalPrecision { get; set; } = 2;

        [JsonProperty("storageCapture")]
        public StorageCaptureConfigurationModel StorageCapture { get; set; } = new StorageCaptureConfigurationModel();
    }
}
