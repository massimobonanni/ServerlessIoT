using CommandLine;
using Microsoft.Azure.Devices.Client;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TelemetrySimulator.TelemetryGenerators;

namespace TelemetrySimulator
{
    class Program
    {
        private static Parameters _parameters;

        private static TelemetryGeneratorConfiguration DefaultTemperatureGeneratorConfiguration =
            new TelemetryGeneratorConfiguration()
            {
                Name = "RandomTelemetryGenerator",
                Configuration = "{'maxValue': 30.0, 'minValue':0.0}"
            };
        private static TelemetryGeneratorConfiguration DefaultHumidityGeneratorConfiguration =
            new TelemetryGeneratorConfiguration()
            {
                Name = "RandomTelemetryGenerator",
                Configuration = "{'maxValue': 100.0, 'minValue':0.0}"
            };

        private static Random rand = new Random(DateTime.Now.Millisecond);

        static async Task Main(string[] args)
        {
            ParserResult<Parameters> result = Parser.Default.ParseArguments<Parameters>(args)
                .WithParsed(parsedParams =>
                {
                    _parameters = parsedParams;
                })
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });

            if (_parameters.IsValid())
                Environment.Exit(1);

            Console.WriteLine("Telemetry simulator.");

            DevicesConfiguration config = await ReadConfigurationAsync(_parameters);

            Console.WriteLine("Press control-C to exit.");

            using var cts = new CancellationTokenSource();

            if (_parameters.ActualDuration > 0)
            {
                cts.CancelAfter(_parameters.ActualDuration * 1000);
            }

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (var device in config.Devices)
            {
                tasks.Add(SendDeviceToCloudMessagesAsync(device, cts.Token));
                await Task.Delay(rand.Next(125, 2000));
            }

            Task.WaitAll(tasks.ToArray());

        }

        private static async Task<DevicesConfiguration> ReadConfigurationAsync(Parameters parameters)
        {
            DevicesConfiguration config = null;
            if (_parameters.IsFileConfig())
            {
                config = await ReadFromFileConfigAsync(parameters);
            }
            else
            {
                config = await ReadFromBlobStorageAsync(parameters);
            }

            foreach (var device in config.Devices)
            {
                if (device.TemperatureGenerator == null)
                {
                    device.TemperatureGenerator = DefaultTemperatureGeneratorConfiguration;
                }
                if (device.HumidityGenerator == null)
                {
                    device.HumidityGenerator = DefaultHumidityGeneratorConfiguration;
                }
            }

            return config;
        }

        private static async Task<DevicesConfiguration> ReadFromBlobStorageAsync(Parameters parameters)
        {
            Console.WriteLine($"Reading configuration from blob '{parameters.BlobUrl}'");

            var uri = new Uri(parameters.BlobUrl);

            using (var client = new HttpClient())
            {
                var configFile = await client.GetStringAsync(uri);
                return JsonSerializer.Deserialize<DevicesConfiguration>(configFile);
            }
        }

        private static async Task<DevicesConfiguration> ReadFromFileConfigAsync(Parameters parameters)
        {
            Console.WriteLine($"Reading configuration from file '{parameters.ConfigFilePath}'");

            var configFile = await File.ReadAllTextAsync(parameters.ConfigFilePath);
            return JsonSerializer.Deserialize<DevicesConfiguration>(configFile);
        }


        private static async Task SendDeviceToCloudMessagesAsync(DeviceConfiguration device, CancellationToken ct)
        {
            DeviceClient deviceClient = null;
            if (!string.IsNullOrWhiteSpace(device.ConnectionString))
            {
                var cs = IotHubConnectionStringBuilder.Create(device.ConnectionString);
                deviceClient = DeviceClient.CreateFromConnectionString(cs.ToString(), TransportType.Mqtt);
            }

            var temperatureGenerator = TelemetryGeneratorFactory.Create(device.TemperatureGenerator);
            var humidityGenerator = TelemetryGeneratorFactory.Create(device.HumidityGenerator);

            while (!ct.IsCancellationRequested)
            {
                var telemetry = new DeviceTelemetry()
                {
                    DeviceId = device.Id,
                    DeviceName = device.Name,
                    Timestamp = DateTimeOffset.Now,
                    Data = new DeviceData()
                    {
                        Humidity = humidityGenerator.GenerateNextValue(),
                        Temperature = temperatureGenerator.GenerateNextValue()
                    }
                };

                string messageBody = JsonSerializer.Serialize(telemetry);
                using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                };


                // Send the telemetry message (if the connectionstring for device is configured)
                if (deviceClient != null)
                    await deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending message: {messageBody}");

                try
                {
                    await Task.Delay(device.PollingIntervalInSec * 1000, ct);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            if (deviceClient != null)
                deviceClient.Dispose();
        }
    }
}
