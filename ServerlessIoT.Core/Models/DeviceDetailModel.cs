using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessIoT.Core.Models
{
    public class DeviceDetailModel : DeviceInfoModel
    {
        [JsonProperty("telemetryHistory")]
        public IEnumerable<DeviceTelemetryModel> TelemetryHistory { get; set; }
    }
}
