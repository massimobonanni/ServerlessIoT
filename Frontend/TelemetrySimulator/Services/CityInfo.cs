using System;

namespace TelemetrySimulator.Services
{
    public class CityInfo
    {
        public string CityCode { get; set; }

        public double Temperature { get; set; }
        public double Humidity { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}