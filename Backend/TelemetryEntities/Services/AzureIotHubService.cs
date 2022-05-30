using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryEntities.Services
{
    public class AzureIotHubService : IIotHubService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AzureIotHubService> logger;

        public AzureIotHubService(IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<AzureIotHubService>();
        }

        public async Task SendMessageToDeviceAsync(string deviceId, string message,
            IDictionary<string, string> properties = null)
        {
            var connectionString = this.configuration["IotHubConnectionString"];
            if (connectionString == null)
                return;

            this.logger.LogInformation($"Sending message to Device {deviceId}");

            using ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var deviceMessage = new Message(Encoding.ASCII.GetBytes(message));
            if (properties != null && properties.Any())
            {
                foreach (var item in properties)
                {
                    deviceMessage.Properties[item.Key] = item.Value;
                }
            }
            deviceMessage.ExpiryTimeUtc = DateTimeOffset.Now.UtcDateTime.AddMinutes(10);

            await serviceClient.SendAsync(deviceId, deviceMessage);
        }
    }
}
