using Newtonsoft.Json;
using ServerlessIoT.Core;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TelemetryDispatcher;

namespace Azure.Messaging.EventHubs.Processor
{
    internal static class ProcessEventArgsExtensions
    {
        public static DeviceTelemetry ToDeviceTelemetry(this ProcessEventArgs @event)
        {
            DeviceTelemetry deviceTelemetry;
            try
            {
                string data = Encoding.UTF8.GetString(@event.Data.Body.ToArray());
                deviceTelemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(data);
                if (deviceTelemetry != null && string.IsNullOrWhiteSpace(deviceTelemetry.DeviceId))
                {
                    if (@event.Data.SystemProperties.ContainsKey(IoTHubConstants.ConnectionDeviceID))
                    {
                        deviceTelemetry.DeviceId = @event.Data.SystemProperties[IoTHubConstants.ConnectionDeviceID]?.ToString();
                        deviceTelemetry.DeviceName = @event.Data.SystemProperties[IoTHubConstants.ConnectionDeviceID]?.ToString();
                    }
                }
                if (deviceTelemetry != null && deviceTelemetry.Timestamp == default)
                {
                    if (@event.Data.SystemProperties.ContainsKey(IoTHubConstants.EnqueuedTime))
                    {
                        if (DateTimeOffset.TryParse(@event.Data.SystemProperties[IoTHubConstants.EnqueuedTime]?.ToString(),
                            null, DateTimeStyles.AssumeUniversal, out var enqueueTime))
                        {
                            deviceTelemetry.Timestamp = enqueueTime;
                        }
                    }
                }
            }
            catch (Exception)
            {
                deviceTelemetry = null;
            }
            return deviceTelemetry;
        }
    }
}
