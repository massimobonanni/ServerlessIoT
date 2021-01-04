using Microsoft.AspNetCore.Http;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEntities.Filters
{
    public class GetDevicesFilters
    {
        public string DeviceNameFilter { get; set; }

        public string DeviceIdFilter { get; set; }


        public static GetDevicesFilters CreateFromHttpRequest(HttpRequest req)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            var filter = new GetDevicesFilters();

            filter.DeviceNameFilter = req.Query["name"];
            filter.DeviceIdFilter = req.Query["id"];

            return filter;
        }

        public bool IsDeviceValid(DeviceInfoModel device)
        {
            var result = true;
            if (this.DeviceNameFilter != null)
            {
                result &= device.DeviceName.Contains(this.DeviceNameFilter,StringComparison.OrdinalIgnoreCase);
            }
            if (this.DeviceIdFilter != null)
            {
                result &= device.DeviceId.Contains(this.DeviceIdFilter, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
