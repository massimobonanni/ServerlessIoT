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
    }
}
