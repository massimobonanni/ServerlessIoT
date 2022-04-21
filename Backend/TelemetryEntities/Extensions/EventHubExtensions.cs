using Newtonsoft.Json;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs
{
    public static class EventHubExtensions
    {
        public static DeviceTelemetry ExtractDeviceTelemetry(this EventData message)
        {
            if (message == null) 
                throw new NullReferenceException(nameof(message));

            var messageBody = Encoding.UTF8.GetString(message.EventBody);
            var telemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(messageBody);
            if (string.IsNullOrWhiteSpace(telemetry.DeviceId))
            {
                telemetry.DeviceId = message.SystemProperties["iothub-connection-device-id"].ToString();
            }
            if (telemetry.Timestamp == DateTimeOffset.MinValue)
            {
                telemetry.Timestamp = message.EnqueuedTime;
            }
            return telemetry;
        }
    }
}
