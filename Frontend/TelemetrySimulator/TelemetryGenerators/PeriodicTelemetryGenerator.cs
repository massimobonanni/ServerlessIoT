using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

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

            [JsonPropertyName("noisePercentage")]
            public double NoisePercentage { get; set; }
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

        private static Random _randGenerator = new Random(DateTime.Now.Millisecond);

        public override Task<double> GenerateNextValueAsync(CancellationToken token)
        {
            if (!_startTime.HasValue)
                _startTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(_randGenerator.Next(0, 1000)));

            var delta = _configuration.MaxValue - _configuration.MinValue;
            var timeFromStart = DateTime.Now.Subtract(_startTime.Value);
            var sinArg = timeFromStart.TotalSeconds * 2.0 * Math.PI / _configuration.PeriodInSec;
            var retVal = delta * (Math.Sin(sinArg) + 1.0) / 2.0 + _configuration.MinValue;
            if (_configuration.NoisePercentage > 0)
            {
                var randSign = (2 * Math.Sign(_randGenerator.Next(-1, 1)) + 1);
                var randNoise = _randGenerator.NextDouble() * _configuration.NoisePercentage / 100.0;
                retVal = retVal + randSign * randNoise * (_configuration.MaxValue - _configuration.MinValue) / 2;
            }
            return Task.FromResult(retVal);
        }
    }
}
