using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetrySimulator.TelemetryGenerators
{
    public abstract class TelemetryGeneratorBase
    {
        public TelemetryGeneratorBase(string jsonConfig)
        {

        }

        public abstract Task<double> GenerateNextValueAsync(CancellationToken token);
    }
}
