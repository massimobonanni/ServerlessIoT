using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEntities.Models
{
    public class DeviceEntityConfiguration
    {
        [JsonProperty("historyRetention")]
        public TimeSpan HistoryRetention { get; set; } = TimeSpan.FromMinutes(10);

        [JsonProperty("temperatureHighThreshold")]
        public double? TemperatureHighThreshold { get; set; }

        [JsonProperty("temperatureLowThreshold")]
        public double? TemperatureLowThreshold { get; set; }

        [JsonProperty("notificationNumber")]
        public string NotificationNumber { get; set; }


        public bool TemperatureHighAlertEnabled()
        {
            return TemperatureHighThreshold.HasValue && !string.IsNullOrWhiteSpace(NotificationNumber);
        }

        public bool TemperatureLowAlertEnabled()
        {
            return TemperatureLowThreshold.HasValue && !string.IsNullOrWhiteSpace(NotificationNumber);
        }
    }
}
