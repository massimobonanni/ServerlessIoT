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
    }
}
