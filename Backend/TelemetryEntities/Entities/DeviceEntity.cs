using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryEntities.Models;

namespace TelemetryEntities.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DeviceEntity
    {
        private readonly ILogger logger;

        public DeviceEntity(ILogger logger)
        {
            this.logger = logger;
            EntityConfig = new DeviceEntityConfiguration();
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

        #endregion [ State ]

        #region [ Behaviour ]
        public void TelemetryReceived(DeviceTelemetry telemetry)
        {
            if (HistoryData == null)
                HistoryData = new Dictionary<DateTimeOffset, DeviceData>();

            HistoryData[telemetry.Timestamp] = telemetry.Data;

            if (LastUpdate < telemetry.Timestamp)
            {
                LastUpdate = telemetry.Timestamp;
                LastData = telemetry.Data;
                DeviceName = telemetry.DeviceName;
            }

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
        #endregion [ Behaviour ]


        [FunctionName(nameof(DeviceEntity))]
        public Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
            => ctx.DispatchAsync<DeviceEntity>(logger);
    }
}
