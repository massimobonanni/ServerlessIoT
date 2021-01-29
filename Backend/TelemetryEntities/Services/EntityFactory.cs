using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryEntities.Entities;

namespace TelemetryEntities.Services
{
    public class EntityFactory : IEntityFactory
    {
        public Task<EntityId> GetEntityIdAsync(string deviceId, CancellationToken token)
        {
            if (deviceId == null)
                throw new ArgumentNullException(nameof(deviceId));

            var deviceEntityName = nameof(StandardDeviceEntity);
            if (deviceId.EndsWith("WeatherDevice"))
            {
                deviceEntityName = nameof(WeatherDeviceEntity);
            }

            return Task.FromResult(new EntityId(deviceEntityName, deviceId));
        }
    }
}
