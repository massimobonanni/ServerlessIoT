﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEntities.Models
{
    public class DeviceEntityConfiguration
    {
        [JsonProperty("historyRetention")]
        public TimeSpan HistoryRetention { get; set; } = TimeSpan.FromMinutes(10);

        [JsonProperty("temperatureHighThreshold")]
        public double? TemperatureHighThreshold { get; set; }

        [JsonProperty("temperatureLowThreshold")]
        public double? TemperatureLowThreshold { get; set; }

        [JsonProperty("notificationNumber")]
        public string NotificationNumber { get; set; }

        [JsonProperty("temperatureDecimalPrecision")]
        public int TemperatureDecimalPrecision { get; set; } = 2;

        [JsonProperty("humidityDecimalPrecision")]
        public int HumidityDecimalPrecision { get; set; } = 2;

        public bool TemperatureHighAlertEnabled()
        {
            return TemperatureHighThreshold.HasValue;
        }

        public bool TemperatureLowAlertEnabled()
        {
            return TemperatureLowThreshold.HasValue;
        }

        [JsonProperty("storageCapture")]
        public StorageCaptureConfiguration StorageCapture { get; set; } = new();

        public bool StorageCaptureEnabled()
        {
            return StorageCapture!= null && StorageCapture.Enabled;
        }
    }

    public class StorageCaptureConfiguration
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty("timeWindowInMinutes")]
        public int TimeWindowInMinutes { get; set; } = 5;

        public bool IsTimeToCapture(DateTimeOffset? lastCapture)
        {
            if (!Enabled)
                return false;

            if (!lastCapture.HasValue)
                return true;

            return DateTimeOffset.Now.Subtract(lastCapture.Value).TotalMinutes >= TimeWindowInMinutes;
        }
    }
}
