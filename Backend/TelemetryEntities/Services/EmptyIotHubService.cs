using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryEntities.Services
{
    public class EmptyIotHubService : IIotHubService
    {
        public Task<bool> InvokeDeviceMethodAsync(string deviceId,
            string methodName, string methodPayload, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SendMessageToDeviceAsync(string deviceId, string message,
            IDictionary<string, string> properties, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
