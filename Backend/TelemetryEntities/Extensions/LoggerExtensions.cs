using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static void LogTelemetryInfo(this ILogger logger, DeviceTelemetry telemetry)
        {
            var deviceId = telemetry?.DeviceId;
            var timestamp = telemetry?.Timestamp;
            logger.LogInformation("Receiving telemetry from device {deviceId} [telemetry timestamp {timestamp}]",deviceId,timestamp);
        }
    }
}
