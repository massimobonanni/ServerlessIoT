using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetrySimulator.Messages
{
    public class OpenWindowMessage
    {
        public WindowNames WindowToOpen { get; set; }

        public object Parameter { get; set; }
    }
}
