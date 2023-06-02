using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryEntities.Models;

namespace TelemetryEntities.Models
{
    internal static class CaptureStoragedataExtensions
    {

        public static string ToCsvString(this StorageCaptureData data, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureInfo.GetCultureInfo("en-US");

            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Timestamp,Temperature,Humidity");
            foreach (var item in data.Data)
                csv.AppendLine(string.Format(culture, "{0},{1},{2}", item.Key, item.Value.Temperature, item.Value.Humidity));

            return csv.ToString();
        }
    }
}
