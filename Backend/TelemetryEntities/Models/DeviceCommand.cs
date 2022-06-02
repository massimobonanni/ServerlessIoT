using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryEntities.Models
{
    public class DeviceMethod
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}
