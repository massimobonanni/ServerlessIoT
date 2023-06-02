using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryEntities.Models;

namespace TelemetryEntities.Services
{
    public class AzureStorageAccountService : IStorageService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AzureStorageAccountService> logger;

        public AzureStorageAccountService(IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<AzureStorageAccountService>();
        }

        private string GetConnectionString()
        {
            return this.configuration["StorageCapture:ConnectionString"];
        }

        private string GetCaptureContainerName()
        {
            return this.configuration["StorageCapture:ContainerName"];
        }

        public async Task<bool> SaveCaptureDataAsync(StorageCaptureData data,
            CancellationToken cancellationToken = default)
        {
            var result = false;

            var connectionString = this.GetConnectionString();
            var containerName = this.GetCaptureContainerName();
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName))
            {
                this.logger.LogWarning("Storage connection string or container name not valid");
                return false;
            }

            try
            {
                var containerClient = new BlobContainerClient(connectionString, containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobName = $"{data.DeviceId}/{DateTimeOffset.Now:yyyyMMdd-HHmmss}.csv";
                this.logger.LogInformation($"Saving capture data to {blobName}");
                var blobClient = containerClient.GetBlobClient(blobName);
                var blobContent = data.ToCsvString();
                var uploadResponse = await blobClient.UploadAsync(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(blobContent)));
                result = true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex,"Error during capture saving");
            }

            return result;
        }
    }
}
