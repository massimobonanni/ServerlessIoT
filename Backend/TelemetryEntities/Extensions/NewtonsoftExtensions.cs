using Newtonsoft.Json.Linq;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Newtonsoft.Json.Linq
{
    public static class NewtonsoftExtensions
    {
        public static DeviceInfoModel ToDeviceInfoModel(this JObject jobject)
        {
            if (jobject == null)
                return null;

            var retVal = new DeviceInfoModel();
            retVal.DeviceName = (string)jobject.Property("deviceName").Value;
            retVal.LastTelemetry = jobject.ToDeviceTelemetryModel();

            return retVal;
        }
        public static DeviceTelemetryModel ToDeviceTelemetryModel(this JObject jobject)
        {
            if (jobject == null)
                return null;

            DeviceTelemetryModel retVal = null;
            var lastTelemetry = jobject.Property("lastData").Value as JObject;
            if (lastTelemetry != null)
            {
                retVal = new DeviceTelemetryModel();
                retVal.Timestamp = (DateTimeOffset)jobject.Property("lastUpdate").Value;
                retVal.Temperature = (double)lastTelemetry.Property("temperature").Value;
                retVal.Humidity = (double)lastTelemetry.Property("humidity").Value;
            }

            return retVal;
        }
    }
}
