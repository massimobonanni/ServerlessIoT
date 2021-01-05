using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceTelemetryModel = TelemetryDashboard.WPF.Models.DeviceTelemetryModel;

namespace TelemetryDashboard.WPF
{
    internal class DeviceTelemetryModelComparer : IEqualityComparer<DeviceTelemetryModel>
    {
        public bool Equals(DeviceTelemetryModel x, DeviceTelemetryModel y)
        {
            if (x == null)
                return false;
            if (y == null)
                return false;
            return x.Timestamp == y.Timestamp;
        }

        public int GetHashCode(DeviceTelemetryModel obj)
        {
            return obj.Timestamp.GetHashCode();
        }
    }
}
