using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryEntities.Models
{
    public class StorageCaptureData
    {
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public IDictionary<DateTimeOffset, DeviceData> Data { get; set; }
    }
}
