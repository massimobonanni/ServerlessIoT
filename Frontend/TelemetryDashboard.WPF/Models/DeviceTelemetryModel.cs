using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryDashboard.WPF.Models
{
    public class DeviceTelemetryModel
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
    }
}
