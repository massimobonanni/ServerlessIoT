using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryDashboard.WPF.Interfaces
{
    interface IContextSettableObject
    {
        Task<bool> SetContextAsync(object sender, object context, CancellationToken cancellationToken);
    }
}
