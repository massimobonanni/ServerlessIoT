using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TelemetrySimulator.TelemetryGenerators
{
    public static class TelemetryGeneratorFactory
    {

        public static TelemetryGeneratorBase Create(TelemetryGeneratorConfiguration config)
        {
            TelemetryGeneratorBase generator = null;
            try
            {
                var generatorType = Type.GetType(config.Name);
                if (generatorType == null)
                    generatorType = Type.GetType($"TelemetrySimulator.TelemetryGenerators.{config.Name}");
                string configJson = null;
                if (config.Configuration is string)
                    configJson = config.Configuration;
                else
                    configJson = System.Text.Json.JsonSerializer.Serialize(config.Configuration);
                generator = (TelemetryGeneratorBase)Activator.CreateInstance(generatorType, configJson);

            }
            catch (Exception)
            {
                generator = null;
            }
            return generator;
        }
    }
}
