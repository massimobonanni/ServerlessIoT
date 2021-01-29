using ServerlessIoT.Core;
using TelemetryEntities.Models;

namespace TelemetryEntities.Entities
{
    public interface IDeviceEntity
    {
        void SetConfiguration(DeviceEntityConfiguration config);
        void TelemetryReceived(DeviceTelemetry telemetry);
    }
}