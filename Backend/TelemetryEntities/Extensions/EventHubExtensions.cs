using Microsoft.Extensions.Logging;
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
        public static DeviceTelemetry ExtractDeviceTelemetry(this EventData message, ILogger logger)
        {
            if (message == null)
                throw new NullReferenceException(nameof(message));

            DeviceTelemetry telemetry = null;

            try
            {
                var messageBody = Encoding.UTF8.GetString(message.EventBody);
                logger?.LogTrace(messageBody);
                telemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(messageBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during telemetry message deserialization");
            }

            if (telemetry != null)
            {
                if (string.IsNullOrWhiteSpace(telemetry.DeviceId))
                    telemetry.DeviceId = message.SystemProperties["iothub-connection-device-id"].ToString();
                if (string.IsNullOrWhiteSpace(telemetry.DeviceName))
                    telemetry.DeviceName = message.SystemProperties["iothub-connection-device-id"].ToString();
                if (telemetry.Timestamp == DateTimeOffset.MinValue)
                    telemetry.Timestamp = message.EnqueuedTime;

            }
            return telemetry;
        }
    }
}
