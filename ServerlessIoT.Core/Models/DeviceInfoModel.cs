using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessIoT.Core.Models
{
    public class DeviceInfoModel
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("lastTelemetry")]
        public DeviceTelemetryModel LastTelemetry { get; set; }
    }
}
