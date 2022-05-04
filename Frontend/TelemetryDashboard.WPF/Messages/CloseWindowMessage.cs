using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryDashboard.WPF.Messages
{
    public class CloseWindowMessage
    {
        public WindowNames WindowToClose { get; set; }

        public object Parameter { get; set; }
    }
}
