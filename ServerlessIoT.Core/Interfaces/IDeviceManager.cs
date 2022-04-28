using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryEntities.Models;

namespace ServerlessIoT.Core.Interfaces
{
    public interface IDeviceManager
    {
        Task<IEnumerable<DeviceInfoModel>> GetDevicesAsync(string filterName, string filterId, CancellationToken token);

        Task<DeviceDetailModel> GetDeviceAsync(string deviceId, CancellationToken token);

        Task<bool> SetDeviceConfigurationAsync(string deviceId, DeviceConfigurationModel configuration, CancellationToken cancellationToken);

        Task<DeviceConfigurationModel> GetDeviceConfigurationAsync(string deviceId, CancellationToken cancellationToken);
    }
}
