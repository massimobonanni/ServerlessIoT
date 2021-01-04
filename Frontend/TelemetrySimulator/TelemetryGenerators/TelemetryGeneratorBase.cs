using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetrySimulator.TelemetryGenerators
{
    public abstract class TelemetryGeneratorBase
    {
        public TelemetryGeneratorBase(string jsonConfig)
        {

        }

        public abstract double GenerateNextValue();
    }
}
