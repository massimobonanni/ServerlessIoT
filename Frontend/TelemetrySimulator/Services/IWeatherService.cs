using System.Threading;
using System.Threading.Tasks;

namespace TelemetrySimulator.Services
{
    public interface IWeatherService
    {
        string ApiKey { get; set; }

        Task<CityInfo> GetCityInfoAsync(string cityCode, CancellationToken token);
    }
}