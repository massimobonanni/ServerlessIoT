using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessIoT.Core.Models
{
    public class DeviceMethodModel
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}
