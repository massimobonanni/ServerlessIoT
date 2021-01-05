using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TelemetrySimulator.TelemetryGenerators
{
    public class PeriodicTelemetryGenerator : TelemetryGeneratorBase
    {
        private class Configuration
        {
            [JsonPropertyName("minValue")]
            public double MinValue { get; set; }

            [JsonPropertyName("maxValue")]
            public double MaxValue { get; set; }

            [JsonPropertyName("periodInSec")]
            public int PeriodInSec { get; set; } = 60;
        }

        private DateTime? _startTime;
        private Configuration _configuration;

        public PeriodicTelemetryGenerator(string jsonConfig) : base(jsonConfig)
        {
            try
            {
                _configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfig);
            }
            catch (Exception)
            {
                _configuration = new Configuration() { MinValue = 0, MaxValue = 10, PeriodInSec = 60 };
            }
        }

        public override double GenerateNextValue()
        {
            if (!_startTime.HasValue)
                _startTime = DateTime.Now;

            var delta = _configuration.MaxValue - _configuration.MinValue;
            var timeFromStart = DateTime.Now.Subtract(_startTime.Value);
            var sinArg = timeFromStart.TotalSeconds * 2.0 * Math.PI / _configuration.PeriodInSec;
            return delta * (Math.Sin(sinArg) + 1.0) / 2.0 + _configuration.MinValue;
        }
    }
}
