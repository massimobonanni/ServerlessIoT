using Newtonsoft.Json.Linq;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    public static class DurableEntityStatusExtensions
    {
        public static DeviceInfoModel ToDeviceInfoModel(this DurableEntityStatus entity)
        {
            if (entity == null)
                return null;

            var retVal = new DeviceInfoModel();
            retVal.DeviceId = entity.EntityId.EntityKey;

            var jobject = entity.State as JObject;
            if (jobject != null)
            {
                retVal.DeviceName = (string)jobject.Property("deviceName").Value;
                retVal.LastTelemetry = entity.ToDeviceTelemetryModel();
            }

            return retVal;
        }
        public static DeviceTelemetryModel ToDeviceTelemetryModel(this DurableEntityStatus entity)
        {
            if (entity == null)
                return null;

            DeviceTelemetryModel retVal = null;

            var jobject = entity.State as JObject;
            if (jobject != null)
            {
                var lastTelemetry = jobject.Property("lastData").Value as JObject;
                if (lastTelemetry != null)
                {
                    retVal = new DeviceTelemetryModel();
                    retVal.Timestamp = (DateTimeOffset)jobject.Property("lastUpdate").Value;
                    retVal.Temperature = (double)lastTelemetry.Property("temperature").Value;
                    retVal.Humidity = (double)lastTelemetry.Property("humidity").Value;
                }
            }

            return retVal;
        }
    }
}
