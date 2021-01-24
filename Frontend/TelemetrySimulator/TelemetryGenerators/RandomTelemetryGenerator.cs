using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetrySimulator.TelemetryGenerators
{
    public class RandomTelemetryGenerator : TelemetryGeneratorBase
    {
        private class Configuration
        {
            [JsonPropertyName("minValue")]
            public double MinValue { get; set; }

            [JsonPropertyName("maxValue")]
            public double MaxValue { get; set; }
        }

        private Configuration _configuration;
        private static Random _randGenerator = new Random(DateTime.Now.Millisecond);

        public RandomTelemetryGenerator(string jsonConfig) : base(jsonConfig)
        {
            try
            {
                _configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfig);
            }
            catch (Exception)
            {
                _configuration = new Configuration() { MinValue = 0, MaxValue = 50 };
            }
        }

        public override Task<double> GenerateNextValueAsync(CancellationToken token)
        {
            var delta = _configuration.MaxValue - _configuration.MinValue;
            return Task.FromResult(_configuration.MinValue + _randGenerator.NextDouble() * delta);
        }
    }
}
