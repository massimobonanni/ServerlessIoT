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
using TelemetryEntities.Externals;
using TelemetryEntities.Models;

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
        #endregion [ State ]

        #region [ Behaviour ]
        public void TelemetryReceived(DeviceTelemetry telemetry)
        {
            if (HistoryData == null)
                HistoryData = new Dictionary<DateTimeOffset, DeviceData>();

            if (telemetry.Timestamp < DateTimeOffset.Now.Subtract(EntityConfig.HistoryRetention))
                return;

            HistoryData[telemetry.Timestamp] = telemetry.Data;

            if (LastUpdate < telemetry.Timestamp)
            {
                LastUpdate = telemetry.Timestamp;
                LastData = telemetry.Data;
                DeviceName = telemetry.DeviceName;
            }

            ClearHistoryData();
            CheckAlert();
        }

        public void SetConfiguration(DeviceEntityConfiguration config)
        {
            this.EntityConfig = config;
        }
        #endregion [ Behaviour ]

        #region [ Private Methods ]
        private void CheckAlert()
        {
            if (this.EntityConfig.TemperatureHighAlertEnabled())
            {
                if (!this.TemperatureHighNotificationFired)
                {
                    if (this.LastData.Temperature > this.EntityConfig.TemperatureHighThreshold)
                    {
                        Entity.Current.StartNewOrchestration(nameof(Orchestrators.SendNotification),
                            new Orchestrators.NotificationData()
                            {
                                DeviceName = this.DeviceName,
                                NotificationNumber = this.EntityConfig.NotificationNumber
                            });

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
                        Entity.Current.StartNewOrchestration(nameof(Orchestrators.SendNotification),
                            new Orchestrators.NotificationData()
                            {
                                DeviceName = this.DeviceName,
                                NotificationNumber = this.EntityConfig.NotificationNumber
                            });

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
        #endregion [ Private Methods ]

        [FunctionName(nameof(StandardDeviceEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
            => ctx.DispatchAsync<StandardDeviceEntity>(logger);
    }
}
