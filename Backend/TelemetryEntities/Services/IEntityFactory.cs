using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryEntities.Services
{
    public interface IEntityFactory
    {
        Task<EntityId> GetEntityIdAsync(string deviceId, CancellationToken token);
    }
}
