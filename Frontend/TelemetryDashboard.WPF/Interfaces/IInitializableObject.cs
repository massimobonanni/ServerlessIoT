using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryDashboard.WPF.Interfaces
{
    interface IInitializableObject
    {
        Task InitializeAsync(object sender, CancellationToken cancellationToken);
    }
}
