﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryEntities.Models;
using TelemetryEntities.Orchestrators;

namespace TelemetryEntities.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StandardDeviceEntity : IDeviceEntity
    {
        private readonly ILogger logger;

        public StandardDeviceEntity(ILogger logger)
        {
            this.logger = logger;
            this.EntityConfig = new DeviceEntityConfiguration();
        }

        #region [ State ]
        [JsonProperty("historyData")]
        public Dictionary<DateTimeOffset, DeviceData> HistoryData { get; set; }

        [JsonProperty("entityConfig")]
        public DeviceEntityConfiguration EntityConfig { get; set; }

        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTimeOffset LastUpdate { get; set; }

        [JsonProperty("lastData")]
        public DeviceData LastData { get; set; }

        [JsonProperty("temperatureHighNotificationFired")]
        public bool TemperatureHighNotificationFired { get; set; } = false;

        [JsonProperty("temperatureLowNotificationFired")]
        public bool TemperatureLowNotificationFired { get; set; } = false;

        [JsonProperty("lastStorageCapture")]
        public DateTimeOffset? LastStorageCapture { get; set; }
        #endregion [ State ]

        #region [ Behaviour ]
        public void TelemetryReceived(DeviceTelemetry telemetry)
        {
            if (HistoryData == null)
                HistoryData = new Dictionary<DateTimeOffset, DeviceData>();

            if (telemetry == null || telemetry.Data == null)
                return;

            if (telemetry.Timestamp < DateTimeOffset.Now.Subtract(EntityConfig.HistoryRetention))
                return;

            NormalizeTelemetryData(telemetry);

            HistoryData[telemetry.Timestamp] = telemetry.Data;

            if (LastUpdate < telemetry.Timestamp)
            {
                LastUpdate = telemetry.Timestamp;
                LastData = telemetry.Data;
                DeviceName = telemetry.DeviceName;
            }

            ClearHistoryData();
            CheckAlert();
            ManageStorageCapture();
        }

        public void SetConfiguration(DeviceEntityConfiguration config)
        {
            if (config == null)
                return;

            if (config.HumidityDecimalPrecision < 0)
                config.HumidityDecimalPrecision = 2;

            if (config.TemperatureDecimalPrecision < 0)
                config.TemperatureDecimalPrecision = 2;

            if (config.StorageCapture == null)
                config.StorageCapture = new StorageCaptureConfiguration();
            else if (config.StorageCapture.TimeWindowInMinutes < 0)
                config.StorageCapture.TimeWindowInMinutes = 5;

            this.EntityConfig = config;
        }
        #endregion [ Behaviour ]

        #region [ Private Methods ]
        private void NormalizeTelemetryData(DeviceTelemetry telemetry)
        {
            telemetry.Data.Temperature = Math.Round(telemetry.Data.Temperature, EntityConfig.TemperatureDecimalPrecision);
            telemetry.Data.Humidity = Math.Round(telemetry.Data.Humidity, EntityConfig.HumidityDecimalPrecision);
        }

        private void ManageStorageCapture()
        {
            if (this.EntityConfig.StorageCaptureEnabled())
            {
                if (this.EntityConfig.StorageCapture.IsTimeToCapture(this.LastStorageCapture))
                {
                    if (!this.LastStorageCapture.HasValue)
                        this.LastStorageCapture = this.HistoryData.Keys.Min();

                    var captureData = new StorageCaptureData()
                    {
                        DeviceId = Entity.Current.EntityKey,
                        DeviceName = this.DeviceName,
                        Data = this.HistoryData
                        .Where(d => d.Key >= this.LastStorageCapture.Value)
                        .ToDictionary(d => d.Key, d => d.Value)
                    };

                    Entity.Current.StartNewOrchestration(
                        nameof(StorageCaptureOrchestrator.SaveCapture), captureData);

                    this.LastStorageCapture = DateTimeOffset.Now;
                }
            }
        }

        private void CheckAlert()
        {
            if (this.EntityConfig.TemperatureHighAlertEnabled())
            {
                if (!this.TemperatureHighNotificationFired)
                {
                    if (this.LastData.Temperature > this.EntityConfig.TemperatureHighThreshold)
                    {
                        Notify(NotificationType.HighTemperature);

                        this.TemperatureHighNotificationFired = true;
                    }
                }
                else
                {
                    if (this.LastData.Temperature < this.EntityConfig.TemperatureHighThreshold)
                    {
                        this.TemperatureHighNotificationFired = false;
                    }
                }
            }

            if (this.EntityConfig.TemperatureLowAlertEnabled())
            {
                if (!this.TemperatureLowNotificationFired)
                {
                    if (this.LastData.Temperature < this.EntityConfig.TemperatureLowThreshold)
                    {
                        Notify(NotificationType.LowTemperature);

                        this.TemperatureLowNotificationFired = true;
                    }
                }
                else
                {
                    if (this.LastData.Temperature > this.EntityConfig.TemperatureLowThreshold)
                    {
                        this.TemperatureLowNotificationFired = false;
                    }
                }
            }
        }

        private void ClearHistoryData()
        {
            var dataToRemove = HistoryData
                 .Where(a => a.Key < DateTimeOffset.Now.Subtract(EntityConfig.HistoryRetention));

            if (dataToRemove.Any())
            {
                foreach (var item in dataToRemove)
                {
                    HistoryData.Remove(item.Key);
                }
            }
        }

        private void Notify(NotificationType notificationType)
        {
            var notificationData = new NotificationData()
            {
                Timestamp = DateTimeOffset.Now,
                DeviceName = this.DeviceName,
                NotificationNumber = this.EntityConfig.NotificationNumber,
                Type = notificationType,
                DeviceId = Entity.Current.EntityKey
            };

            switch (notificationType)
            {
                case NotificationType.HighTemperature:
                    notificationData.CurrentValue = this.LastData.Temperature;
                    notificationData.ThresholdValue = this.EntityConfig.TemperatureHighThreshold;
                    break;
                case NotificationType.LowTemperature:
                    notificationData.CurrentValue = this.LastData.Temperature;
                    notificationData.ThresholdValue = this.EntityConfig.TemperatureLowThreshold;
                    break;
                case NotificationType.HighHumidity:
                    notificationData.CurrentValue = this.LastData.Humidity;
                    break;
                case NotificationType.LowHumidity:
                    notificationData.CurrentValue = this.LastData.Humidity;
                    break;
                default:
                case NotificationType.Unknown:
                    break;
            }

            Entity.Current.StartNewOrchestration(nameof(NotificationOrchestrator.SendNotification), notificationData);

        }
        #endregion [ Private Methods ]

        [FunctionName(nameof(StandardDeviceEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
            => ctx.DispatchAsync<StandardDeviceEntity>(logger);
    }
}
