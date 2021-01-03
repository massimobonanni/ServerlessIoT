using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessIoT.Core.Interfaces
{
    public interface ITelemetryManager
    {
        Task<bool> SendTelemetryToDeviceAsync(DeviceTelemetry telemetry, CancellationToken cancellationToken);

        Task<IEnumerable<DeviceInfoModel>> GetDevicesAsync(CancellationToken token);

        Task<DeviceInfoModel> GetDeviceAsync(string deviceId, CancellationToken token);
    }
}
