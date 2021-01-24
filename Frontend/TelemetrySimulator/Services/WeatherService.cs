using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetrySimulator.Services
{
    public class WeatherService : IWeatherService
    {
        private string baseUri = "https://api.openweathermap.org/data/2.5/weather";

        public string ApiKey { get; set; }

        private Uri GetApiUrl(string cityCode)
        {
            return new Uri($"{baseUri}?q={cityCode}&appId={ApiKey}&units=metric");
        }


        public async Task<CityInfo> GetCityInfoAsync(string cityCode, CancellationToken token)
        {
            CityInfo city = null;
            string response = null;
            using (var client = new HttpClient())
            {
                var httpResponse = await client.GetAsync(GetApiUrl(cityCode), token);
                if (httpResponse != null)
                {
                    response = await httpResponse.Content.ReadAsStringAsync();
                }
            }

            if (response != null)
            {
                var responseObj = JsonConvert.DeserializeObject<WeatherData>(response);
                if (responseObj.cod == 200)
                {
                    var cityData = responseObj.main;

                    city = new CityInfo()
                    {
                        CityCode = cityCode,
                        Temperature = cityData.temp,
                        Humidity = cityData.humidity,
                        Timestamp = responseObj.dt.ToUtcDateTimeOffset()
                    };
                }
            }
            return city;
        }
    }

}
