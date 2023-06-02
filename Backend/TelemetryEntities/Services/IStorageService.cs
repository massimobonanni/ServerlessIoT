using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryEntities.Models;

namespace TelemetryEntities.Services
{
    public interface IStorageService
    {
        Task<bool> SaveCaptureDataAsync(StorageCaptureData data, CancellationToken cancellationToken = default);
    }
}
