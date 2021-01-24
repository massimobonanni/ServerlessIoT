using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetrySimulator.Services;

namespace TelemetrySimulator.TelemetryGenerators
{
    public class OpenWeatherTelemetryGenerator : TelemetryGeneratorBase
    {
        private class Configuration
        {
            [JsonProperty("cityCode")]
            public string CityCode { get; set; }

            [JsonProperty("apiKey")]
            public string ApiKey { get; set; }

            [JsonProperty("weatherData")]
            public WeatherData WeatherData { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        private enum WeatherData
        {
            [EnumMember(Value = "temperature")]
            Temperature,
            [EnumMember(Value = "humidity")]
            Humidity
        }

        private IWeatherService _weatherService = new WeatherService();
        private Configuration _configuration;

        public OpenWeatherTelemetryGenerator(string jsonConfig) : base(jsonConfig)
        {
            try
            {
                _configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfig);
                this._weatherService.ApiKey = _configuration.ApiKey;
            }
            catch (Exception)
            {
                _configuration = new Configuration() { };
            }
        }

        public override async Task<double> GenerateNextValueAsync(CancellationToken token)
        {
            double retval = 0;
            if (this._configuration.ApiKey != null)
            {
                CityInfo weatherData;
                try
                {
                    weatherData = await this._weatherService.GetCityInfoAsync(this._configuration.CityCode, token);
                }
                catch (Exception)
                {
                    weatherData = null;
                }
                if (weatherData != null)
                {
                    switch (this._configuration.WeatherData)
                    {
                        case WeatherData.Temperature:
                            retval = weatherData.Temperature;
                            break;
                        case WeatherData.Humidity:
                            retval = weatherData.Humidity;
                            break;
                        default:
                            break;
                    }
                }
            }
            return retval;
        }
    }
}
