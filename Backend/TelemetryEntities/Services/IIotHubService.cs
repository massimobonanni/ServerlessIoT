using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryEntities.Services
{
    public interface IIotHubService
    {
        Task SendMessageToDeviceAsync(string deviceId, string message, 
            IDictionary<string, string> properties);
        Task<bool> InvokeDeviceMethodAsync(string deviceId, string methodName, 
            string methodPayload);
    }
}
