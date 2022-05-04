using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreDeviceInfoModel = ServerlessIoT.Core.Models.DeviceInfoModel;

namespace TelemetryDashboard.WPF.Models
{
    public class DeviceInfoModel
    {
        public DeviceInfoModel()
        {

        }
        public DeviceInfoModel(CoreDeviceInfoModel device)
        {
            this.DeviceInfo=device;
        }

        public CoreDeviceInfoModel DeviceInfo { get; set; }

        public DateTimeOffset? LastTelemetryLocalTimestamp
        {
            get
            {
                if (this.DeviceInfo != null && this.DeviceInfo.LastTelemetry != null)
                    return this.DeviceInfo.LastTelemetry.Timestamp.ToLocalTime();
                return null;
            }
        }

    }
}
