using Newtonsoft.Json;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Messaging.EventHubs.Consumer
{
    internal static class PartitionEventExtensions
    {
        public static DeviceTelemetry ToDeviceTelemetry(this PartitionEvent @event)
        {
            DeviceTelemetry deviceTelemetry;
            try
            {
                string data = Encoding.UTF8.GetString(@event.Data.Body.ToArray());
                deviceTelemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(data);
            }
            catch (Exception)
            {
                deviceTelemetry = null;
            }
            return deviceTelemetry;
        }
    }
}
