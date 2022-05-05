using Newtonsoft.Json.Linq;
using ServerlessIoT.Core;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelemetryEntities.Models;

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

        public static DeviceDetailModel ToDeviceDetailModel(this JObject jobject)
        {
            if (jobject == null)
                return null;

            var retVal = new DeviceDetailModel();
            retVal.DeviceName = jobject.Property("deviceName").Value?.ToString();
            retVal.LastTelemetry = jobject.ToDeviceTelemetryModel();
            var historyData = jobject.Property("historyData").Value.ToObject<Dictionary<DateTimeOffset, DeviceData>>();

            if (historyData != null && historyData.Any())
            {
                retVal.TelemetryHistory = historyData
                    .Select(kv => new DeviceTelemetryModel()
                    {
                        Timestamp = kv.Key,
                        Temperature = kv.Value == null ? double.NaN : kv.Value.Temperature,
                        Humidity = kv.Value == null ? double.NaN : kv.Value.Humidity
                    })
                    .OrderBy(v => v.Timestamp)
                    .ToList();
            }

            return retVal;
        }


        public static DeviceEntityConfiguration ExtractDeviceConfiguration(this JObject jobject)
        {
            if (jobject == null)
                return null;

            var retVal = jobject.Property("entityConfig").Value.ToObject<DeviceEntityConfiguration>();

            return retVal;
        }
    }
}
